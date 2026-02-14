namespace DigitalisationManager.Data.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    using DigitalisationManager.Data.Models.Enums;

    using static DigitalisationManager.GCommon.ValidationConstants.Fund;

    public class Fund
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public FundType FundType { get; set; }

        [Required]
        [StringLength(CodeMax, MinimumLength = CodeMin)]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(TitleMax, MinimumLength = TitleMin)]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMax)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } 

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
