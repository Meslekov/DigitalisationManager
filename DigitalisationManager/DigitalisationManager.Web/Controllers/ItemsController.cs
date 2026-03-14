namespace DigitalisationManager.Web.Controllers
{
    using DigitalisationManager.Services.Core;
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Web.ViewModels.Item;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize]
    public class ItemsController : Controller
    {
        private readonly IItemService itemService;
        private readonly IDigitalFileService dgFileService;

        public ItemsController(IItemService itemService, IDigitalFileService dgFileService)
        {
            this.itemService = itemService;
            this.dgFileService = dgFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? fundId, string? q)
        {
            var model = await itemService.GetIndexAsync(fundId, q);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await itemService.GetDetailsAsync(id);
            if (model is null) return NotFound();

            model.Files = await dgFileService.ListByItemAsync(id);
            model.FilesCount = model.Files.Count;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? fundId)
        {
            var model = await itemService.GetCreateAsync(fundId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await itemService.GetCreateAsync(model.FundId);
                return View(model);
            }

            try
            {
                var newId = await itemService.CreateAsync(model);
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(ItemFormViewModel.InventoryNumber),
                    "Inventory number must be unique within the selected fund.");

                model = await itemService.GetCreateAsync(model.FundId);
                model.InventoryNumber = model.InventoryNumber;
                model.Description = model.Description;
                model.DocumentDateText = model.DocumentDateText;
                model.Status = model.Status;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await itemService.GetForEditAsync(id);
            if (model is null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ItemFormViewModel model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var vm = await itemService.GetForEditAsync(id);
                return View(vm ?? model);
            }

            var result = await itemService.UpdateAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Unable to update item.");
                var vm = await itemService.GetForEditAsync(id);
                return View(vm ?? model);
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await itemService.GetForDeleteAsync(id);
            if (model is null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await itemService.DeleteAsync(id);

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
