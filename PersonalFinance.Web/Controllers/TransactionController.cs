using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Transaction;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;
        // private readonly ICategoryService _categoryService;

        public TransactionController(
            ITransactionService transactionService,
            IAccountService accountService
            /* ICategoryService categoryService */)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            // _categoryService = categoryService;
        }

        private async Task PrepareDropdownsAsync(Guid userId)
        {
            var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
            // var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);

            ViewBag.Accounts = new SelectList(accounts, "Id", "Name");
            // ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await PrepareDropdownsAsync(userId);

            return View(new TransactionFormViewModel { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionFormViewModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!ModelState.IsValid)
            {
                await PrepareDropdownsAsync(userId);
                return View(model);
            }

            try
            {
                await _transactionService.CreateTransactionAsync(model, userId);
                TempData["SuccessMessage"] = "บันทึกรายการสำเร็จ";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PrepareDropdownsAsync(userId);
                return View(model);
            }
        }
    }
}