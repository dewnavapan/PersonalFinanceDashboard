using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Goal;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;

        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var goals = await _goalService.GetGoalsAsync(userId);
            return View(goals);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new GoalFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GoalFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _goalService.CreateGoalAsync(model, userId);

            TempData["SuccessMessage"] = "สร้างเป้าหมายการออมสำเร็จ";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFund(Guid id, decimal addAmount)
        {
            if (addAmount <= 0) return RedirectToAction(nameof(Index));

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _goalService.AddFundToGoalAsync(id, addAmount, userId);

            TempData["SuccessMessage"] = "อัปเดตยอดเงินออมสำเร็จ";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _goalService.DeleteGoalAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}