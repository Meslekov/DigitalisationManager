namespace DigitalisationManager.Web.ViewModels.DigitalFile
{
    public class DigitalFileUploadResultViewModel
    {
        public string FileName { get; set; } = null!;

        public bool Success { get; set; }

        public string? Error { get; set; }
    }
}