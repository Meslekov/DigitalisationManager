namespace DigitalisationManager.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Web.ViewModels.Funds;

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
    }
}
