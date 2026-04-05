namespace DigitalisationManager.Web.ViewModels.DigitalFile
{
    public class BatchDigitalFileUploadResultViewModel
    {
        public int TotalCount { get; set; }

        public int SuccessCount { get; set; }

        public int FailedCount { get; set; }

        public List<DigitalFileUploadResultViewModel> Results { get; set; } = new();
    }
}