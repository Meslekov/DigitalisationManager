namespace DigitalisationManager.Data.Models.Identity
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class ApplicationRole : IdentityRole<Guid>
    {
        [PersonalData]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string? Lable { get; set; }
    }
}
