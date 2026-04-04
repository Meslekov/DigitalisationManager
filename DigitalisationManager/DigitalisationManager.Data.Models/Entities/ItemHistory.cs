namespace DigitalisationManager.Data.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static DigitalisationManager.GCommon.ValidationConstants.ItemHistory;

    public class ItemHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(ActionMax, MinimumLength = ActionMin)]
        public string Action { get; set; } = null!;

        [StringLength(DescriptionMax)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        public virtual Item Item { get; set; } = null!;
    }
}