namespace DigitalisationManager.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using DigitalisationManager.Services.Core.Contracts;

    public class DigitalFilesController : AdminBaseController
    {
        private readonly IDigitalFileService digitalFileService;

        public DigitalFilesController(IDigitalFileService digitalFileService)
        {
            this.digitalFileService = digitalFileService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int itemId, IFormFile file)
        {
            if (file is null)
            {
                TempData["Error"] = "Please select a file.";
                return RedirectToAction("Details", "Items", new { id = itemId });
            }

            (bool Success, string? Error) result = await digitalFileService.UploadAsync(itemId, file);

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
            }
            else
            {
                TempData["Success"] = "File uploaded successfully.";
            }

            return RedirectToAction("Details", "Items", new { id = itemId });
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var result = await digitalFileService.DownloadOriginalAsync(id);
            if (result is null)
            {
                return NotFound();
            }

            return File(result.Value.Content, result.Value.ContentType, result.Value.DownloadName);
        }

        [HttpPost]
        public async Task<IActionResult> SetDownloadAllowed(int id, int itemId, bool isAllowed)
        {
            (bool Success, string? Error) result = await digitalFileService.SetDownloadAllowedAsync(id, isAllowed);

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
            }
            else
            {
                TempData["Success"] = isAllowed
                    ? "File download enabled for users."
                    : "File download disabled for users.";
            }

            return RedirectToAction("Details", "Items", new { area = "Admin", id = itemId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int itemId)
        {
            (bool Success, string? Error) result = await digitalFileService.DeleteAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
            }
            else
            {
                TempData["Success"] = "File deleted.";
            }

            return RedirectToAction("Details", "Items", new { id = itemId });
        }
    }
}