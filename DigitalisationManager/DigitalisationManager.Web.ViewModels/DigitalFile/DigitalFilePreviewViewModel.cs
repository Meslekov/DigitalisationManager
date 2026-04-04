namespace DigitalisationManager.Web.ViewModels.DigitalFile
{
    public class DigitalFilePreviewViewModel
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public string OriginalFileName { get; set; } = null!;

        public string PreviewImageUrl { get; set; } = null!;

        public int Position { get; set; }

        public int TotalCount { get; set; }

        public int? PreviousFileId { get; set; }

        public int? NextFileId { get; set; }

        public bool CanDownloadOriginal { get; set; }

        public bool CanDownloadPreview { get; set; }
    }
}