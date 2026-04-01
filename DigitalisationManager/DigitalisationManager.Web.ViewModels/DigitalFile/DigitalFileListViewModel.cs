namespace DigitalisationManager.Web.ViewModels.DigitalFile
{
    public class DigitalFileListViewModel
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = null!;
        public long SizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? ChecksumSha256 { get; set; }
        public bool IsDownloadAllowed { get; set; }
    }
}
