using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class NetWorthController : Controller
    {
        private readonly INetWorthService _netWorthService;

        public NetWorthController(INetWorthService netWorthService)
        {
            _netWorthService = netWorthService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var summary = await _netWorthService.GetNetWorthSummaryAsync(userId);
            return View(summary);
        }
    }
}