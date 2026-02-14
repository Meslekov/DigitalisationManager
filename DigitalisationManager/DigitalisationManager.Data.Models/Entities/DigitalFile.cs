namespace DigitalisationManager.Data.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static DigitalisationManager.GCommon.ValidationConstants.DigitalFile;
    public class DigitalFile
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(OriginalNameMax)]
        public string OriginalFileName { get; set; } = null!;

        [Required]
        [MaxLength(StoredNameMax)]
        public string StoredFileName { get; set; } = null!;

        [Required]
        [MaxLength(PathMax)]
        public string RelativePath { get; set; } = null!;

        [Required]
        public long SizeBytes { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; }

        [MaxLength(ChecksumMax)]
        public string? ChecksumSha256 { get; set; }

        [Required]
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        public virtual Item Item { get; set; } = null!;
    }
}