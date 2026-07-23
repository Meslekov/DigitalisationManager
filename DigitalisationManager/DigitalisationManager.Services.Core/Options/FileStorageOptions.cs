using System.ComponentModel.DataAnnotations;

namespace DigitalisationManager.Services.Core.Options
{
    public class FileStorageOptions
    {
        [Required]
        public string RootFolder { get; set; } = "Storage";

        [Range(1, long.MaxValue)]
        public long MaxTiffUploadSizeBytes { get; set; }
    }
}
