using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.Web.Controllers
{
    [Authorize]
    public class ImportExportController : Controller
    {
        private readonly IImportExportService _importExportService;

        public ImportExportController(IImportExportService importExportService)
        {
            _importExportService = importExportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fileBytes = await _importExportService.ExportTransactionsToCsvAsync(userId);
            return File(fileBytes, "text/csv", $"Transactions_{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "กรุณาเลือกไฟล์ CSV";
                return RedirectToAction(nameof(Index));
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "รองรับเฉพาะไฟล์ .csv เท่านั้น";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _importExportService.ImportTransactionsFromCsvAsync(file.OpenReadStream(), userId);

                if (result.errors.Any())
                {
                    ViewBag.Errors = result.errors;
                    ViewBag.SuccessCount = result.successCount;
                    return View("Index");
                }

                TempData["SuccessMessage"] = $"นำเข้าข้อมูลสำเร็จจำนวน {result.successCount} รายการ";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}