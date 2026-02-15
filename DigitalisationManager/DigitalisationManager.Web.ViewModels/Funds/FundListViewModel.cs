namespace DigitalisationManager.Web.ViewModels.Funds
{
    public class FundListViewModel
    {
        public int Id { get; set; }
        public string FundType { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int ItemsCount { get; set; }
    }
}
