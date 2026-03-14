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
    using System.Collections.Generic;
    using System.Linq;

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

            vm.Funds = await GetFundsSelectListAsync();
            return vm;
        }

        public async Task<int> CreateAsync(ItemFormViewModel model)
        {
            Item entity = new Item
            {
                FundId = model.FundId,
                InventoryNumber = model.InventoryNumber.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                DocumentDateText = string.IsNullOrWhiteSpace(model.DocumentDateText) ? null : model.DocumentDateText.Trim(),
                Status = model.Status,
                CreatedAt = DateTime.UtcNow
            };

            context.Items.Add(entity);

            await context.SaveChangesAsync();

            return entity.Id;
        }


        public async Task<ItemDetailsViewModel?> GetDetailsAsync(int id)
        {
            return await context.Items
               .AsNoTracking()
               .Include(i => i.Fund)
               .Where(i => i.Id == id)
               .Select(i => new ItemDetailsViewModel
               {
                   Id = i.Id,
                   FundId = i.FundId,
                   FundCode = i.Fund.Code,
                   InventoryNumber = i.InventoryNumber,
                   Description = i.Description,
                   DocumentDateText = i.DocumentDateText,
                   Status = i.Status,
                   CreatedAt = i.CreatedAt,
                   FilesCount = i.DigitalFiles.Count
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

            IQueryable<Data.Models.Entities.Item> query = context.Items
                .AsNoTracking()
                .Include(i => i.Fund)
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
                    (i.DocumentDateText != null && i.DocumentDateText.Contains(trimmedQ)));
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
                    InventoryNumber = i.InventoryNumber,
                    Description = i.Description,
                    DocumentDateText = i.DocumentDateText,
                    Status = i.Status,
                    FilesCount = i.DigitalFiles.Count
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
            var vm = await context.Items
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new ItemFormViewModel
                {
                    Id = i.Id,
                    FundId = i.FundId,
                    InventoryNumber = i.InventoryNumber,
                    Description = i.Description,
                    DocumentDateText = i.DocumentDateText,
                    Status = i.Status
                })
                .FirstOrDefaultAsync();

            if (vm is null) return null;

            vm.Funds = await GetFundsSelectListAsync();
            return vm;
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(ItemFormViewModel model)
        {
            if (model.Id is null) return (false, "Item not found.");

            var entity = await context.Items.FirstOrDefaultAsync(i => i.Id == model.Id.Value);
            if (entity is null) return (false, "Item not found.");

            entity.FundId = model.FundId;
            entity.InventoryNumber = model.InventoryNumber.Trim();
            entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            entity.DocumentDateText = string.IsNullOrWhiteSpace(model.DocumentDateText) ? null : model.DocumentDateText.Trim();
            entity.Status = model.Status;

            try
            {
                await context.SaveChangesAsync();
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
            var entity = await context.Items
                .Include(i => i.DigitalFiles)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity is null) return (false, "Item not found.");

            if (entity.DigitalFiles.Any())
                return (false, "Cannot delete this item because it has uploaded TIFF files. Delete the files first.");

            context.Items.Remove(entity);
            await context.SaveChangesAsync();
            return (true, null);
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
    }
}
