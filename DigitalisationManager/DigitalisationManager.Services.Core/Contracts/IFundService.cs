using DigitalisationManager.Web.ViewModels.Funds;

namespace DigitalisationManager.Services.Core.Contracts
{
    public interface IFundService
    {
        Task<FundQueryViewModel> GetIndexAsync(string? q, int page, int pageSize);

        Task<FundDetailsViewModel?> GetDetailsAsync(int id);

        Task<int> CreateAsync(FundFormViewModel model);

        Task<FundFormViewModel?> GetForEditAsync(int id);

        Task<bool> UpdateAsync(FundFormViewModel model);

        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }
}