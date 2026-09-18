using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // เพิ่มบรรทัดนี้
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Transaction;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService; // เพิ่ม
        private readonly ICategoryService _categoryService; // เพิ่ม

        public TransactionController(
            ITransactionService transactionService,
            IAccountService accountService,
            ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
        }

        private async Task LoadDropdownDataAsync(Guid userId)
        {
            var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
            var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);

            ViewBag.Accounts = new SelectList(accounts, "Id", "Name");
            // ส่ง Categories ไปเป็น List ปกติเพื่อใช้กรองด้วย JavaScript
            ViewBag.Categories = categories;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await LoadDropdownDataAsync(userId);
            return View(new TransactionFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionFormViewModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(userId); // โหลดใหม่ถ้ากรอกผิด
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
                await LoadDropdownDataAsync(userId);
                return View(model);
            }
        }
    }
}