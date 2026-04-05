namespace DigitalisationManager.Web.Areas.Admin.Controllers
{
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Web.ViewModels.Item;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class ItemsController : AdminBaseController
    {
        private readonly IItemService itemService;
        private readonly IDigitalFileService dgFileService;

        public ItemsController(IItemService itemService, IDigitalFileService dgFileService)
        {
            this.itemService = itemService;
            this.dgFileService = dgFileService;
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

            model.Files = await dgFileService.ListByItemAsync(id);
            model.FilesCount = model.Files.Count;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? fundId)
        {
            ItemFormViewModel model = await itemService.GetCreateAsync(fundId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await itemService.PopulateFormModelAsync(model);
                return View(model);
            }

            try
            {
                int newId = await itemService.CreateAsync(model);
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await itemService.PopulateFormModelAsync(model);
                return View(model);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(ItemFormViewModel.InventoryNumber),
                    "Inventory number must be unique within the selected fund.");

                await itemService.PopulateFormModelAsync(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ItemFormViewModel? model = await itemService.GetForEditAsync(id);
            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemFormViewModel model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await itemService.PopulateFormModelAsync(model);
                return View(model);
            }

            (bool Success, string? Error) result = await itemService.UpdateAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Unable to update item.");
                await itemService.PopulateFormModelAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ItemDetailsViewModel? model = await itemService.GetForDeleteAsync(id);
            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            (bool Success, string? Error) result = await itemService.DeleteAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Item deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}