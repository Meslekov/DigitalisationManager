namespace DigitalisationManager.Web.ViewModels.Funds
{
    using DigitalisationManager.GCommon.Paging;

    public class FundQueryViewModel
    {
        public string? Q { get; set; }

        public PagedResult<FundListViewModel> Results { get; set; } = new PagedResult<FundListViewModel>();
    }
}