using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Portfolio;
using System.Security.Claims;


namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var summary = await _portfolioService.GetPortfolioSummaryAsync(userId);
            return View(summary);
        }
        [HttpGet]
        public IActionResult CreateTransaction()
        {
            var model = new InvestmentTransactionFormViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransaction(InvestmentTransactionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

                await _portfolioService.AddTransactionAsync(model, userId);

                TempData["SuccessMessage"] = "บันทึกข้อมูลการลงทุนสำเร็จ";
                return RedirectToAction(nameof(Index)); // กลับไปหน้าพอร์ตหลัก
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"เกิดข้อผิดพลาด: {ex.Message}");
                return View(model);
            }
        }
    }
}