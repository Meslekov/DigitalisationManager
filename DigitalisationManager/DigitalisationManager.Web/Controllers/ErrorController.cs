namespace DigitalisationManager.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        [Route("Error/{statusCode:int}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return statusCode switch
            {
                403 => View("Forbidden"),
                404 => View("NotFound"),
                500 => View("ServerError"),
                _ => View("ServerError")
            };
        }

        [Route("Error/403")]
        public IActionResult Forbidden()
        {
            return View("Forbidden");
        }

        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }

        [Route("Error/500")]
        public IActionResult ServerError()
        {
            return View("ServerError");
        }
    }
}