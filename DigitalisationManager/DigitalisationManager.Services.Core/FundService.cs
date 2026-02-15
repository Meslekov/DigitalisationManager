using DigitalisationManager.Data;
using DigitalisationManager.Services.Core.Contracts;
using DigitalisationManager.Web.ViewModels.Funds;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DigitalisationManager.Services.Core
{
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
    }
}
