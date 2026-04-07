namespace DigitalisationManager.Web.Areas.User.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static DigitalisationManager.GCommon.ApplicationConstants;

    [Area("User")]
    [Authorize(Roles = $"{RoleNames.User}, {RoleNames.Administrator}, {RoleNames.Manager}")]
    public class UserBaseController : Controller
    {
    }
}
