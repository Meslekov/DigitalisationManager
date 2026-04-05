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
            var files = await digitalFileService.DownloadPreviewAsync(id);
            if (files is null)
            {
                return NotFound();
            }

            return File(files.Value.Content, files.Value.ContentType, files.Value.DownloadName);
        }
    }
}