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

            using var stream = file.OpenReadStream();

            var result = await digitalFileService.UploadTiffAsync(
                itemId,
                stream,
                file.FileName,
                file.Length);

            if (!result.Success)
                TempData["Error"] = result.Error;

            return RedirectToAction("Details", "Items", new { id = itemId });
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var (found, originalName, stream) = await digitalFileService.OpenDownloadStreamAsync(id);
            if (!found || stream is null) return NotFound();

            // "image/tiff" is correct for TIFF
            return File(stream, "image/tiff", originalName);
        }

        [HttpPost]
        public async Task<IActionResult> SetDownloadAllowed(int id, int itemId, bool isAllowed)
        {
            var result = await digitalFileService.SetDownloadAllowedAsync(id, isAllowed);

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
            var result = await digitalFileService.DeleteAsync(id, itemId);
            if (!result.Success)
                TempData["Error"] = result.Error;
            else
                TempData["Success"] = "File deleted.";

            return RedirectToAction("Details", "Items", new { id = itemId });
        }
    }
}
