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

            // ตัด ModelState validation ที่เป็น Navigation Property ออกชั่วคราว
            ModelState.Remove("User");

            if (!ModelState.IsValid) return View(model);

            // บันทึกผ่าน DbContext โดยตรงหรือผ่าน AccountService (เพื่อความรวดเร็ว ใช้ผ่าน Context หรือเพิ่ม Method ใน Service ได้ครับ)
            // ตัวอย่างนี้แนะนำให้เพิ่มใน AccountService หรือบันทึกผ่าน Context ได้เลยครับ
            return RedirectToAction(nameof(Index));
        }
    }
}