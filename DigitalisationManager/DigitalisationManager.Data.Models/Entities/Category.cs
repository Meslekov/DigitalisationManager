namespace DigitalisationManager.Data.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using static DigitalisationManager.GCommon.ValidationConstants.Category;

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(NameMax, MinimumLength = NameMin)]
        public string Name { get; set; } = null!;

        [StringLength(DescriptionMax)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
