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
        [StringLength(OriginalNameMax)]
        public string OriginalFileName { get; set; } = null!;

        [Required]
        [StringLength(StoredNameMax)]
        public string OriginalStoredFileName { get; set; } = null!;

        [Required]
        [StringLength(PathMax)]
        public string OriginalRelativePath { get; set; } = null!;

        [Required]
        [StringLength(ContentTypeMax)]
        public string OriginalContentType { get; set; } = null!;

        [Required]
        public long OriginalSizeBytes { get; set; }

        [Required]
        [StringLength(ChecksumMax)]
        public string OriginalChecksumSha256 { get; set; } = null!;

        [Required]
        [StringLength(StoredNameMax)]
        public string PreviewStoredFileName { get; set; } = null!;

        [Required]
        [StringLength(PathMax)]
        public string PreviewRelativePath { get; set; } = null!;

        [Required]
        [StringLength(ContentTypeMax)]
        public string PreviewContentType { get; set; } = null!;

        [Required]
        public long PreviewSizeBytes { get; set; }

        [Required]
        public bool IsDownloadAllowed { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; }

        [Required]
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        public virtual Item Item { get; set; } = null!;
    }
}