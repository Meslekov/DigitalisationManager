namespace DigitalisationManager.Web.ViewModels.Item
{
    using DigitalisationManager.Web.ViewModels.Shared;

    public class ItemQueryViewModel
    {
        public int? FundId { get; set; }
        public string? Q { get; set; }

        public IEnumerable<DropdownOptionViewModel> Funds { get; set; } = Enumerable.Empty<DropdownOptionViewModel>();
        public List<ItemListViewModel> Results { get; set; } = new();
    }
}
