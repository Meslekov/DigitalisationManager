namespace DigitalisationManager.Services.Core
{
    using DigitalisationManager.Data;
    using DigitalisationManager.Data.Models.Entities;
    using DigitalisationManager.GCommon.Paging;
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Services.Core.Helpers;
    using DigitalisationManager.Web.ViewModels.Item;
    using DigitalisationManager.Web.ViewModels.Shared;
    using Microsoft.EntityFrameworkCore;

    public class ItemService : IItemService
    {
        private readonly DigitalisationManagerDbContext context;

        public ItemService(DigitalisationManagerDbContext context)
        {
            this.context = context;
        }

        public async Task<ItemFormViewModel> GetCreateAsync(int? fundId)
        {
            ItemFormViewModel vm = new ItemFormViewModel
            {
                FundId = fundId ?? 0
            };

            await PopulateFormModelAsync(vm);

            return vm;
        }

        public async Task PopulateFormModelAsync(ItemFormViewModel model)
        {
            model.Funds = await GetFundsSelectListAsync();
            model.Categories = await GetCategoriesSelectListAsync();
        }

        public async Task<int> CreateAsync(ItemFormViewModel model)
        {
            Item entity = new Item
            {
                FundId = model.FundId,
                CategoryId = model.CategoryId,
                InventoryNumber = model.InventoryNumber.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                DocumentDateText = string.IsNullOrWhiteSpace(model.DocumentDateText) ? null : model.DocumentDateText.Trim(),
                Status = model.Status,
                CreatedAt = DateTime.UtcNow
            };

            context.Items.Add(entity);

            await context.SaveChangesAsync();

            await AppendHistoryAsync(entity.Id, "Item created", "Initial item record was created.");

            return entity.Id;
        }

        public async Task<ItemDetailsViewModel?> GetDetailsAsync(int id)
        {
            return await context.Items
                .AsNoTracking()
                .Include(i => i.Fund)
                .Include(i => i.Category)
                .Include(i => i.ItemHistories)
                .Where(i => i.Id == id)
                .Select(i => new ItemDetailsViewModel
                {
                    Id = i.Id,
                    FundId = i.FundId,
                    FundCode = i.Fund.Code,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category.Name,
                    InventoryNumber = i.InventoryNumber,
                    Description = i.Description,
                    DocumentDateText = i.DocumentDateText,
                    Status = i.Status,
                    CreatedAt = i.CreatedAt,
                    FilesCount = i.DigitalFiles.Count,
                    HistoryEntries = i.ItemHistories
                        .OrderByDescending(h => h.CreatedAt)
                        .Select(h => new ItemHistoryListViewModel
                        {
                            Id = h.Id,
                            Action = h.Action,
                            Description = h.Description,
                            CreatedAt = h.CreatedAt
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ItemQueryViewModel> GetIndexAsync(int? fundId, string? q, int page, int pageSize)
        {
            int normalizedPage = PagingHelper.NormalizePage(page);
            int normalizedPageSize = PagingHelper.NormalizePageSize(pageSize);

            string? trimmedQ = string.IsNullOrWhiteSpace(q) ? null : q.Trim();

            List<DropdownOptionViewModel> funds = await context.Funds
                .AsNoTracking()
                .OrderBy(f => f.Code)
                .Select(f => new DropdownOptionViewModel
                {
                    Value = f.Id.ToString(),
                    Text = $"{f.Code} - {f.Title}"
                })
                .ToListAsync();

            IQueryable<Item> query = context.Items
                .AsNoTracking()
                .Include(i => i.Fund)
                .Include(i => i.Category)
                .Include(i => i.DigitalFiles)
                .AsQueryable();

            if (fundId.HasValue)
            {
                query = query.Where(i => i.FundId == fundId.Value);
            }

            if (!string.IsNullOrWhiteSpace(trimmedQ))
            {
                query = query.Where(i =>
                    i.InventoryNumber.Contains(trimmedQ) ||
                    (i.Description != null && i.Description.Contains(trimmedQ)) ||
                    (i.DocumentDateText != null && i.DocumentDateText.Contains(trimmedQ)) ||
                    i.Category.Name.Contains(trimmedQ));
            }

            int totalCount = await query.CountAsync();
            int totalPages = totalCount == 0
                ? 0
                : (int)Math.Ceiling(totalCount / (double)normalizedPageSize);

            if (totalPages > 0 && normalizedPage > totalPages)
            {
                normalizedPage = totalPages;
            }

            int skip = (normalizedPage - 1) * normalizedPageSize;

            List<ItemListViewModel> results = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip(skip)
                .Take(normalizedPageSize)
                .Select(i => new ItemListViewModel
                {
                    Id = i.Id,
                    FundId = i.FundId,
                    FundCode = i.Fund.Code,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category.Name,
                    InventoryNumber = i.InventoryNumber,
                    Description = i.Description,
                    DocumentDateText = i.DocumentDateText,
                    Status = i.Status,
                    FilesCount = i.DigitalFiles.Count,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

            return new ItemQueryViewModel
            {
                FundId = fundId,
                Q = trimmedQ,
                Funds = funds,
                Results = new PagedResult<ItemListViewModel>
                {
                    Page = normalizedPage,
                    PageSize = normalizedPageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Items = results
                }
            };
        }

        public async Task<ItemFormViewModel?> GetForEditAsync(int id)
        {
            ItemFormViewModel? vm = await context.Items
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new ItemFormViewModel
                {
                    Id = i.Id,
                    FundId = i.FundId,
                    CategoryId = i.CategoryId,
                    InventoryNumber = i.InventoryNumber,
                    Description = i.Description,
                    DocumentDateText = i.DocumentDateText,
                    Status = i.Status
                })
                .FirstOrDefaultAsync();

            if (vm is null)
            {
                return null;
            }

            await PopulateFormModelAsync(vm);

            return vm;
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(ItemFormViewModel model)
        {
            if (model.Id is null)
            {
                return (false, "Item not found.");
            }

            Item? entity = await context.Items.FirstOrDefaultAsync(i => i.Id == model.Id.Value);
            if (entity is null)
            {
                return (false, "Item not found.");
            }

            string? historyDescription = BuildUpdateHistoryDescription(entity, model);

            entity.FundId = model.FundId;
            entity.CategoryId = model.CategoryId;
            entity.InventoryNumber = model.InventoryNumber.Trim();
            entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            entity.DocumentDateText = string.IsNullOrWhiteSpace(model.DocumentDateText) ? null : model.DocumentDateText.Trim();
            entity.Status = model.Status;

            try
            {
                await context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(historyDescription))
                {
                    await AppendHistoryAsync(entity.Id, "Item updated", historyDescription);
                }

                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Inventory number must be unique within the selected fund.");
            }
        }

        public async Task<ItemDetailsViewModel?> GetForDeleteAsync(int id)
            => await GetDetailsAsync(id);

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            Item? entity = await context.Items
                .Include(i => i.DigitalFiles)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity is null)
            {
                return (false, "Item not found.");
            }

            if (entity.DigitalFiles.Any())
            {
                return (false, "Cannot delete this item because it has uploaded TIFF files. Delete the files first.");
            }

            context.Items.Remove(entity);
            await context.SaveChangesAsync();

            return (true, null);
        }

        private async Task AppendHistoryAsync(int itemId, string action, string? description)
        {
            ItemHistory historyEntry = new ItemHistory
            {
                ItemId = itemId,
                Action = action,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            context.ItemHistories.Add(historyEntry);
            await context.SaveChangesAsync();
        }

        private string? BuildUpdateHistoryDescription(Item entity, ItemFormViewModel model)
        {
            List<string> changes = new List<string>();

            if (entity.FundId != model.FundId)
            {
                changes.Add("Fund changed.");
            }

            if (entity.CategoryId != model.CategoryId)
            {
                changes.Add("Category changed.");
            }

            if (entity.InventoryNumber != model.InventoryNumber.Trim())
            {
                changes.Add("Inventory number updated.");
            }

            string? newDescription = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            if (entity.Description != newDescription)
            {
                changes.Add("Description updated.");
            }

            string? newDocumentDateText = string.IsNullOrWhiteSpace(model.DocumentDateText) ? null : model.DocumentDateText.Trim();
            if (entity.DocumentDateText != newDocumentDateText)
            {
                changes.Add("Document date updated.");
            }

            if (entity.Status != model.Status)
            {
                changes.Add("Status updated.");
            }

            return changes.Count == 0
                ? null
                : string.Join(" ", changes);
        }

        private async Task<List<DropdownOptionViewModel>> GetFundsSelectListAsync()
        {
            return await context.Funds
                .AsNoTracking()
                .OrderBy(f => f.Code)
                .Select(f => new DropdownOptionViewModel
                {
                    Value = f.Id.ToString(),
                    Text = $"{f.Code} - {f.Title}"
                })
                .ToListAsync();
        }

        private async Task<List<DropdownOptionViewModel>> GetCategoriesSelectListAsync()
        {
            return await context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new DropdownOptionViewModel
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }
    }
}