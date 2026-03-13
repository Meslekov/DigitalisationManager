namespace DigitalisationManager.Web.ViewModels.Item
{
    using DigitalisationManager.GCommon.Enums;
    using DigitalisationManager.GCommon.Paging;
    using DigitalisationManager.Web.ViewModels.DigitalFile;

    public class ItemDetailsViewModel
    {
        public int Id { get; set; }

        public int FundId { get; set; }

        public string FundCode { get; set; } = null!;

        public string InventoryNumber { get; set; } = null!;

        public string? Description { get; set; }

        public string? DocumentDateText { get; set; }

        public ItemStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int FilesCount { get; set; }

        public PagedResult<DigitalFileListViewModel> Files { get; set; } = new PagedResult<DigitalFileListViewModel>();
    }
}