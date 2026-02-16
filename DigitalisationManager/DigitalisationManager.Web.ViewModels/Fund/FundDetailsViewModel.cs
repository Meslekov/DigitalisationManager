using DigitalisationManager.GCommon.Enums;

namespace DigitalisationManager.Web.ViewModels.Funds
{
    public class FundDetailsViewModel
    {
        public int Id { get; set; }
        public FundType FundType { get; set; }
        public string Code { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ItemsCount { get; set; }
    }
}
