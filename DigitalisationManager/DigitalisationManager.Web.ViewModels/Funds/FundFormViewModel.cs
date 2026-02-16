
namespace DigitalisationManager.Web.ViewModels.Funds
{
    using System.ComponentModel.DataAnnotations;

    using DigitalisationManager.Data.Models.Enums;

    using static DigitalisationManager.GCommon.ValidationConstants.Fund;

    public class FundFormViewModel
    {
        public int? Id { get; set; } // null for Create, has value for Edit

        [Required]
        public FundType FundType { get; set; }

        [Required, StringLength(CodeMax, MinimumLength = CodeMin)]
        public string Code { get; set; } = null!;

        [Required, StringLength(TitleMax, MinimumLength = TitleMin)]
        public string Title { get; set; } = null!;

        [StringLength(DescriptionMax)]
        public string? Description { get; set; }
    }
}
