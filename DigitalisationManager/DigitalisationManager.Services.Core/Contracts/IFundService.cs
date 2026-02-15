using DigitalisationManager.Web.ViewModels.Funds;

namespace DigitalisationManager.Services.Core.Contracts
{
    public interface IFundService
    {
            Task<List<FundListViewModel>> GetAllFundsAsync();
    }
}
