namespace DigitalisationManager.Web.ViewModels.Item
{
    using DigitalisationManager.GCommon.Enums;

    public class ItemListViewModel
    {
        public int Id { get; set; }

        public int FundId { get; set; }
        public string FundCode { get; set; } = null!;

        public string InventoryNumber { get; set; } = null!;
        public string? Description { get; set; }
        public string? DocumentDateText { get; set; }

        public ItemStatus Status { get; set; }

        public int FilesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
