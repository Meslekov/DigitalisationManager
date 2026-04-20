namespace DigitalisationManager.Data.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using DigitalisationManager.GCommon.Enums;

    using static DigitalisationManager.GCommon.ValidationConstants.Item;

    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(InventoryMax, MinimumLength = InventoryMin)]
        public string InventoryNumber { get; set; } = null!;

        [Required]
        [MaxLength(DocumentDateMax)]
        public string? DocumentDateText { get; set; }

        [StringLength(DescriptionMax)]
        public string? Description { get; set; }

        [Required]
        public ItemStatus Status { get; set; } = ItemStatus.New;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [ForeignKey(nameof(Fund))]
        public int FundId { get; set; }

        public virtual Fund Fund { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;

        public int? ArchiveLocationId { get; set; }

        [ForeignKey(nameof(ArchiveLocationId))]
        public virtual ArchiveLocation? ArchiveLocation { get; set; }

        public virtual ICollection<DigitalFile> DigitalFiles { get; set; } = new List<DigitalFile>();
        
        public virtual ICollection<ItemHistory> ItemHistories { get; set; } = new List<ItemHistory>();
    }
}