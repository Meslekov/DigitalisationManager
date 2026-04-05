namespace DigitalisationManager.Web.Areas.Admin.Controllers
{
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Web.ViewModels.DigitalFile;
    using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Upload(int itemId, List<IFormFile> files)
        {
            BatchDigitalFileUploadResultViewModel result = await digitalFileService.UploadAsync(itemId, files);

            if (result.SuccessCount > 0 && result.FailedCount == 0)
            {
                TempData["Success"] = $"{result.SuccessCount} file(s) uploaded successfully.";
            }
            else if (result.SuccessCount > 0 && result.FailedCount > 0)
            {
                string failedFiles = string.Join("; ",
                    result.Results
                        .Where(r => !r.Success)
                        .Select(r => $"{r.FileName}: {r.Error}"));

                TempData["Warning"] = $"{result.SuccessCount} file(s) uploaded, {result.FailedCount} failed. {failedFiles}";
            }
            else
            {
                string failedFiles = string.Join("; ",
                    result.Results
                        .Where(r => !r.Success)
                        .Select(r => $"{r.FileName}: {r.Error}"));

                TempData["Error"] = failedFiles;
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