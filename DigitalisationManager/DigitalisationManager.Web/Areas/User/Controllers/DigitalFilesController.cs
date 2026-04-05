namespace DigitalisationManager.Web.Areas.User.Controllers
{
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Web.ViewModels.DigitalFile;
    using Microsoft.AspNetCore.Mvc;

    public class DigitalFilesController : UserBaseController
    {
        private readonly IDigitalFileService digitalFileService;

        public DigitalFilesController(IDigitalFileService digitalFileService)
        {
            this.digitalFileService = digitalFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            DigitalFilePreviewViewModel? model = await digitalFileService.GetPreviewPageAsync(
                id,
                enforceUserAccess: true,
                canDownloadOriginal: false,
                canDownloadPreview: true,
                backToItemDetailsArea: "User");

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        [Route("/User/DigitalFiles/PreviewImage/{id:int}")]
        public async Task<IActionResult> PreviewImage(int id)
        {
            var result = await digitalFileService.GetPreviewImageAsync(id, enforceUserAccess: true);
            if (result is null)
            {
                return NotFound();
            }

            return File(result.Value.Content, result.Value.ContentType);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var result = await digitalFileService.DownloadPreviewAsync(id, enforceUserAccess: true);
            if (result is null)
            {
                return NotFound();
            }

            return File(result.Value.Content, result.Value.ContentType, result.Value.DownloadName);
        }
    }
}