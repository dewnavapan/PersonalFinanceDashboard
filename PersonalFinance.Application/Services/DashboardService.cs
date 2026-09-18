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

        public async Task<DashboardViewModel> GetDashboardSummaryAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            // หาต้นเดือนและสิ้นเดือนปัจจุบัน
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // 1. คำนวณยอดเงินรวมทุกบัญชี
            var totalBalance = await _context.Accounts
                .Where(a => a.UserId == userId)
                .SumAsync(a => a.Balance);

            // 2. ดึงธุรกรรมเฉพาะเดือนปัจจุบัน (ใช้ Include เพื่อดึงชื่อ Category มาด้วย)
            var monthlyTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= firstDayOfMonth && t.Date <= lastDayOfMonth)
                .ToListAsync();

            // 3. คำนวณรายรับ-รายจ่ายเดือนนี้
            var monthlyIncome = monthlyTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var monthlyExpense = monthlyTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            // 4. จัดกลุ่มรายจ่ายตามหมวดหมู่ สำหรับทำกราฟ (เช่น อาหาร 5000, เดินทาง 2000)
            var expenseByCategory = monthlyTransactions
                .Where(t => t.Type == TransactionType.Expense && t.CategoryId != null)
                .GroupBy(t => t.Category!.Name)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            return new DashboardViewModel
            {
                TotalBalance = totalBalance,
                MonthlyIncome = monthlyIncome,
                MonthlyExpense = monthlyExpense,
                ExpenseByCategory = expenseByCategory
            };
        }
    }
}