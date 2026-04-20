namespace DigitalisationManager.Data.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class ArchiveLocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string? Room { get; set; }

        [MaxLength(50)]
        public string? Shelf { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}