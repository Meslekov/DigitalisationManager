namespace DigitalisationManager.Web.Areas.User.Controllers
{
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Web.ViewModels.Item;
    using Microsoft.AspNetCore.Mvc;

    public class ItemsController : UserBaseController
    {
        private readonly IItemService itemService;
        private readonly IDigitalFileService digitalFileService;

        public ItemsController(
            IItemService itemService,
            IDigitalFileService digitalFileService)
        {
            this.itemService = itemService;
            this.digitalFileService = digitalFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? fundId, string? q, int page = 1, int pageSize = 20)
        {
            ItemQueryViewModel model = await itemService.GetIndexAsync(fundId, q, page, pageSize);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ItemDetailsViewModel? model = await itemService.GetDetailsAsync(id);
            if (model is null)
            {
                return NotFound();
            }

            model.Files = await digitalFileService.ListByItemAsync(id);
            model.FilesCount = model.Files.Count;

            return View(model);
        }
    }
}
