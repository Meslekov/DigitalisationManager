namespace DigitalisationManager.Web.Areas.Admin.Controllers
{
    using DigitalisationManager.GCommon;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;


    [Area("Admin")]
    [Authorize(Roles = ApplicationConstants.RoleNames.Administrator)]
    public class AdminBaseController : Controller
    {
    }
}