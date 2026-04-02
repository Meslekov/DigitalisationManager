namespace DigitalisationManager.Web.Areas.User.Controllers
{
    using DigitalisationManager.Services.Core.Contracts;
    using Microsoft.AspNetCore.Mvc;

    public class DigitalFilesController : UserBaseController
    {
        private readonly IDigitalFileService digitalFileService;

        public DigitalFilesController(IDigitalFileService digitalFileService)
        {
            this.digitalFileService = digitalFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var (found, originalName, stream) = await digitalFileService.OpenUserDownloadStreamAsync(id);
            if (!found || stream is null)
            {
                return NotFound();
            }

            return File(stream, "image/tiff", originalName);
        }
    }
}
