using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        public IActionResult Index()
        {
            // หน้าชั่วคราวก่อนเข้าสู่ Phase งบประมาณเต็มตัว
            return View();
        }
    }
}