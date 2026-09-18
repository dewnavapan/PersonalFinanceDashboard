using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Investment;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class InvestmentController : Controller
    {
        private readonly IInvestmentService _investmentService;

        public InvestmentController(IInvestmentService investmentService)
        {
            _investmentService = investmentService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var portfolio = await _investmentService.GetPortfolioAsync(userId);
            return View(portfolio);
        }

        [HttpGet]
        public IActionResult Create() => View(new InvestmentFormViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvestmentFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _investmentService.SaveInvestmentAsync(model, userId);

            TempData["SuccessMessage"] = "บันทึกสินทรัพย์สำเร็จ";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrice(Guid id, decimal newPrice)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _investmentService.UpdateMarketPriceAsync(id, newPrice, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _investmentService.DeleteInvestmentAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}