namespace DigitalisationManager.Web.ViewModels.DigitalFile
{
    public class DigitalFileListViewModel
    {
        public int Id { get; set; }

        public string OriginalFileName { get; set; } = null!;

        public string OriginalContentType { get; set; } = null!;

        public long OriginalSizeBytes { get; set; }

        public string PreviewContentType { get; set; } = null!;

        public long PreviewSizeBytes { get; set; }

        public bool IsDownloadAllowed { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}