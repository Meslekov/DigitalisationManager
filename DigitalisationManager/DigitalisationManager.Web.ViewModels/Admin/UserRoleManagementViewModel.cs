namespace DigitalisationManager.Web.ViewModels.Admin
{
    public class UserRoleManagementViewModel
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string SelectedRole { get; set; } = string.Empty;

        public IEnumerable<string> AvailableRoles { get; set; } = new List<string>();
    }
}