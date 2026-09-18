using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Core.Entities;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
            return View(accounts);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.UserId = userId;

            // ตัด ModelState validation ที่เป็น Navigation Property ออก
            ModelState.Remove("User");

            if (!ModelState.IsValid) return View(model);

            // เรียกใช้ Service เพื่อบันทึกลง Database
            await _accountService.CreateAccountAsync(model);

            TempData["SuccessMessage"] = "เพิ่มบัญชีใหม่เรียบร้อยแล้ว";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _accountService.DeleteAccountAsync(id, userId);
            TempData["SuccessMessage"] = "ลบบัญชีเรียบร้อยแล้ว";
            return RedirectToAction(nameof(Index));
        }
    }
}