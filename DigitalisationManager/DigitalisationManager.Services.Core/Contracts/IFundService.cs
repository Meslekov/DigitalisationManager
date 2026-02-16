using DigitalisationManager.Web.ViewModels.Funds;

namespace DigitalisationManager.Services.Core.Contracts
{
    public interface IFundService
    {
            Task<List<FundListViewModel>> GetAllFundsAsync();

            Task<FundDetailsViewModel?> GetDetailsAsync(int id);
            
            Task<int> CreateAsync(FundFormViewModel model);
            
            Task<FundFormViewModel?> GetForEditAsync(int id);
            
            Task<bool> UpdateAsync(FundFormViewModel model);
            
            Task<(bool Success, string? Error)> DeleteAsync(int id);
    }
}
