namespace DigitalisationManager.Web.ViewModels.Item
{
    using System.ComponentModel.DataAnnotations;

    using DigitalisationManager.GCommon.Enums;

    using static DigitalisationManager.GCommon.ValidationConstants.Item;

    using DigitalisationManager.Web.ViewModels.Shared;

    public class ItemFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Fund")]
        public int FundId { get; set; }

        public List<DropdownOptionViewModel> Funds { get; set; } = new();

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public List<DropdownOptionViewModel> Categories { get; set; } = new();


        [Required, StringLength(InventoryMax, MinimumLength = InventoryMin)]
        [Display(Name = "Inventory number")]
        public string InventoryNumber { get; set; } = null!;

        [StringLength(DescriptionMax)]
        public string? Description { get; set; }

        [StringLength(DocumentDateMax)]
        [Display(Name = "Document date (text)")]
        public string? DocumentDateText { get; set; }

        [Required]
        public ItemStatus Status { get; set; } = ItemStatus.New;
    }
}
