using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Budget;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;

        public BudgetController(IBudgetService budgetService, ICategoryService categoryService)
        {
            _budgetService = budgetService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var now = DateTime.Today;
            var budgets = await _budgetService.GetBudgetProgressAsync(userId, now.Month, now.Year);

            ViewBag.CurrentMonth = now.ToString("MMMM yyyy");
            return View(budgets);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);

            // กรองเฉพาะหมวดหมู่รายจ่าย
            ViewBag.ExpenseCategories = new SelectList(categories.Where(c => c.Type == "Expense"), "Id", "Name");
            return View(new BudgetFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BudgetFormViewModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);
                ViewBag.ExpenseCategories = new SelectList(categories.Where(c => c.Type == "Expense"), "Id", "Name");
                return View(model);
            }

            await _budgetService.CreateBudgetAsync(model, userId);
            TempData["SuccessMessage"] = "ตั้งค่างบประมาณเรียบร้อยแล้ว";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _budgetService.DeleteBudgetAsync(id, userId);
            TempData["SuccessMessage"] = "ลบงบประมาณเรียบร้อยแล้ว";
            return RedirectToAction(nameof(Index));
        }
    }
}