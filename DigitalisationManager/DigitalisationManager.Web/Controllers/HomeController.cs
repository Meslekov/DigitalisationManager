
namespace DigitalisationManager.Web.Controllers
{
    using DigitalisationManager.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrator"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                if (User.IsInRole("User"))
                {
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
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
