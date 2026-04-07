namespace DigitalisationManager.Web.Areas.Manager.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static DigitalisationManager.GCommon.ApplicationConstants;

    [Area("Manager")]
    [Authorize(Roles = $"{RoleNames.Manager}, {RoleNames.Administrator}")]
    public class ManagerBaseController : Controller
    { 
    }
}
