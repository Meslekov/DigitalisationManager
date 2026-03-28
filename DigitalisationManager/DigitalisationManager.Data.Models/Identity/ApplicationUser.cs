namespace DigitalisationManager.Data.Models.Identity
{  
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class ApplicationUser : IdentityUser<Guid>
    {
        [PersonalData]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [PersonalData]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string? LastName { get; set; }
    }
}
