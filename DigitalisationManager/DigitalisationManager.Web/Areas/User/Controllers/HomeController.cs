namespace DigitalisationManager.Web.Areas.User.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;

    using DigitalisationManager.Web.ViewModels;

    public class HomeController : UserBaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
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
