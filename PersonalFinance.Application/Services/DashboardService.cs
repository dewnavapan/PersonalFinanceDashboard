using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Dashboard;
using PersonalFinance.Core.Enums;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardSummaryAsync(Guid userId, int month, int year)
        {
            var model = new DashboardViewModel();

            // 1. คำนวณ Total Balance & Net Worth (จากบัญชีทั้งหมด)
            var accounts = await _context.Accounts
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .ToListAsync();

            model.TotalBalance = accounts.Sum(a => a.Balance);
            model.NetWorth = model.TotalBalance; // MVP: สำหรับตอนนี้ Net Worth = รวมยอดเงินทุกบัญชี (อนาคตจะหักลบ Liabilities)

            // 2. ดึง Transaction ของเดือนที่เลือก
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();

            model.TotalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            model.TotalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            // 3. คำนวณ Saving Rate = (Income - Expense) / Income * 100
            if (model.TotalIncome > 0)
            {
                var savings = model.TotalIncome - model.TotalExpense;
                model.SavingRate = savings > 0 ? (savings / model.TotalIncome) * 100 : 0;
            }

            // 4. เตรียมข้อมูลสำหรับ Chart (Expense by Category)
            var expenseByCategory = transactions
                .Where(t => t.Type == TransactionType.Expense && t.CategoryId != null)
                .GroupBy(t => t.Category!.Name)
                .Select(g => new { Category = g.Key, Amount = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Amount)
                .ToList();

            model.ExpenseCategoryLabelsJson = JsonSerializer.Serialize(expenseByCategory.Select(x => x.Category));
            model.ExpenseCategoryDataJson = JsonSerializer.Serialize(expenseByCategory.Select(x => x.Amount));

            // 5. Rule-Based Smart Insights
            if (model.TotalExpense > model.TotalIncome && model.TotalIncome > 0)
            {
                model.SmartInsights.Add("⚠️ Cash Flow เดือนนี้ติดลบ (รายจ่ายสูงกว่ารายรับ) ควรตรวจสอบค่าใช้จ่ายที่ไม่จำเป็น");
            }
            if (model.SavingRate > 20)
            {
                model.SmartInsights.Add("🌟 ยอดเยี่ยม! อัตราการออมของคุณสูงกว่า 20% ของรายได้");
            }
            if (expenseByCategory.Any() && expenseByCategory.First().Amount > (model.TotalIncome * 0.5m))
            {
                model.SmartInsights.Add($"💡 หมวดหมู่ '{expenseByCategory.First().Category}' ใช้เงินไปเกินครึ่งของรายรับเดือนนี้");
            }

            return model;
        }
    }
}