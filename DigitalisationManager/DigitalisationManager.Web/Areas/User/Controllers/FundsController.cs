
namespace DigitalisationManager.Web.Areas.User.Controllers
{
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Web.ViewModels.Funds;

    using Microsoft.AspNetCore.Mvc;

    public class FundsController : UserBaseController
    {
        private readonly IFundService fundService;

        public FundsController(IFundService fundService)
        {
            this.fundService = fundService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 20)
        {
            FundQueryViewModel model = await fundService.GetIndexAsync(q, page, pageSize);

            return View(model);
        }
    }
}
