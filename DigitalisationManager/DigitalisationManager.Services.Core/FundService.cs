namespace DigitalisationManager.Services.Core
{
    using Microsoft.EntityFrameworkCore;

    using DigitalisationManager.Data;
    using DigitalisationManager.Data.Models.Entities;

    using DigitalisationManager.Services.Core.Contracts;

    using DigitalisationManager.Web.ViewModels.Funds;
    using DigitalisationManager.Web.ViewModels.Shared;

    public class FundService : IFundService
    {
        private readonly DigitalisationManagerDbContext context;

        public FundService(DigitalisationManagerDbContext context)
        {
            this.context = context;
        }

        public async Task<List<FundListViewModel>> GetAllFundsAsync()
        {
          return await context.Funds
                .AsNoTracking()
                .OrderBy(f => f.Code)
                .Select(f => new FundListViewModel
                {
                    Id = f.Id,
                    FundType = f.FundType.ToString(),
                    Code = f.Code,
                    Title = f.Title,
                    ItemsCount = f.Items.Count
                })
                .ToListAsync();
        }

        public async Task<FundDetailsViewModel?> GetDetailsAsync(int id)
        {
            return await context.Funds
                .AsNoTracking()
                .Where(f => f.Id == id)
                .Select(f => new FundDetailsViewModel
                {
                    Id = f.Id,
                    FundType = f.FundType,
                    Code = f.Code,
                    Title = f.Title,
                    Description = f.Description,
                    CreatedAt = f.CreatedAt,
                    ItemsCount = f.Items.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(FundFormViewModel model)
        {
            Fund entity = new Fund
            {
                FundType = model.FundType,
                Code = model.Code.Trim(),
                Title = model.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            context.Funds.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<FundFormViewModel?> GetForEditAsync(int id)
        {
            return await context.Funds
                 .AsNoTracking()
                 .Where(f => f.Id == id)
                 .Select(f => new FundFormViewModel
                 {
                     Id = f.Id,
                     FundType = f.FundType,
                     Code = f.Code,
                     Title = f.Title,
                     Description = f.Description
                 })
                 .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(FundFormViewModel model)
        {
            if (model.Id is null) return false;

            Fund? entity = await context.Funds.FirstOrDefaultAsync(f => f.Id == model.Id.Value);
            if (entity is null) return false;

            entity.FundType = model.FundType;
            entity.Code = model.Code.Trim();
            entity.Title = model.Title.Trim();
            entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            Fund? fund = await context.Funds
                  .Include(f => f.Items)
                  .FirstOrDefaultAsync(f => f.Id == id);

            if (fund is null) return (false, "Fund not found.");

            if (fund.Items.Any())
                return (false, "Cannot delete this fund because it contains items. Delete the items first.");

            context.Funds.Remove(fund);
            await context.SaveChangesAsync();

            return (true, null);
        }
    }
}
