namespace DigitalisationManager.Web.Areas.Admin.Controllers
{
    using DigitalisationManager.Data.Models.Identity;
    using DigitalisationManager.GCommon;
    using DigitalisationManager.Web.ViewModels.Admin;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UsersController : AdminBaseController
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = userManager.Users.OrderBy(u => u.Email).ToList();

            List<UserRoleManagementViewModel> model = new();

            foreach (ApplicationUser user in users)
            {
                IList<string> roles = await userManager.GetRolesAsync(user);

                model.Add(new UserRoleManagementViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SelectedRole = roles.FirstOrDefault() ?? ApplicationConstants.RoleNames.User,
                    AvailableRoles = new[]
                    {
                        ApplicationConstants.RoleNames.User,
                        ApplicationConstants.RoleNames.Manager,
                        ApplicationConstants.RoleNames.Administrator
                    }
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(Guid id)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                return NotFound();
            }

            IList<string> roles = await userManager.GetRolesAsync(user);

            UserRoleManagementViewModel model = new UserRoleManagementViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SelectedRole = roles.FirstOrDefault() ?? ApplicationConstants.RoleNames.User,
                AvailableRoles = new[]
                {
                    ApplicationConstants.RoleNames.User,
                    ApplicationConstants.RoleNames.Manager,
                    ApplicationConstants.RoleNames.Administrator
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(UserRoleManagementViewModel model)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(model.UserId.ToString());
            if (user is null)
            {
                return NotFound();
            }

            if (user.Email == "admin@digitalisationmanager.local" &&
                model.SelectedRole != ApplicationConstants.RoleNames.Administrator)
            {
                ModelState.AddModelError(string.Empty, "The seeded main administrator cannot be demoted.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = new[]
                {
                    ApplicationConstants.RoleNames.User,
                    ApplicationConstants.RoleNames.Manager,
                    ApplicationConstants.RoleNames.Administrator
                };

                return View(model);
            }

            IList<string> currentRoles = await userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                await userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await userManager.AddToRoleAsync(user, model.SelectedRole);

            return RedirectToAction(nameof(Index));
        }
    }
}