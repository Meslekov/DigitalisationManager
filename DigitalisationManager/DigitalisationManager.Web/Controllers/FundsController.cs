namespace DigitalisationManager.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using DigitalisationManager.Services.Core.Contracts;
   
    using DigitalisationManager.Web.ViewModels.Funds;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class FundsController : Controller
    {
        private readonly IFundService fundService;

        public FundsController(IFundService fundService)
        {
            this.fundService = fundService;
        }

        public async Task<IActionResult> Index()
        {
            List<FundListViewModel> model = await fundService.GetAllFundsAsync();
            
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            FundDetailsViewModel? model = await fundService.GetDetailsAsync(id);
            if (model is null) return NotFound();

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new FundFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(FundFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            int newId = await fundService.CreateAsync(model);
            return RedirectToAction(nameof(Details), new { id = newId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            FundFormViewModel? model = await fundService.GetForEditAsync(id);
            if (model is null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FundFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            bool ok = await fundService.UpdateAsync(model);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            FundDetailsViewModel? model = await fundService.GetDetailsAsync(id);
            if (model is null) return NotFound();

            return View(model); 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await fundService.DeleteAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Fund deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
