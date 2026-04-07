
namespace DigitalisationManager.Web.Controllers
{
    using DigitalisationManager.GCommon;
    using DigitalisationManager.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    using static DigitalisationManager.GCommon.ApplicationConstants;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole(RoleNames.Administrator))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            if (User.IsInRole(RoleNames.Manager))
            {
                return RedirectToAction("Index", "Home", new { area = "Manager" });
            }

            if (User.IsInRole(RoleNames.User))
            {
                return RedirectToAction("Index", "Home", new { area = "User" });
            }


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
