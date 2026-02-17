namespace DigitalisationManager.Services.Core.Options
{
    public class FileStorageOptions
    {
        public string RootFolder { get; set; } = "Storage";

        // 500 MB default. Configure in appsettings.Development.json.
        public long MaxTiffUploadSizeBytes { get; set; } = 524_288_000;
    }
}
