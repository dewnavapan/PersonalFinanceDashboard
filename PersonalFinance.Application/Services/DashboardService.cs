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
        private readonly INetWorthService _netWorthService;

        public DashboardService(ApplicationDbContext context, INetWorthService netWorthService)
        {
            _context = context;
            _netWorthService = netWorthService;
        }

        // 1. เปลี่ยนชื่อ Method ให้ตรงกับ Interface
        public async Task<DashboardViewModel> GetDashboardSummaryAsync(Guid userId)
        {
            // 2. ลบ throw new NotImplementedException(); ออกไปแล้ว

            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var model = new DashboardViewModel();

            // 1. Total Balance
            model.TotalBalance = await _context.Accounts
                .Where(a => a.UserId == userId && a.Balance > 0)
                .SumAsync(a => a.Balance);

            // 2. Net Worth
            var netWorthSummary = await _netWorthService.GetNetWorthSummaryAsync(userId);
            model.NetWorth = netWorthSummary.NetWorth;

            // 3. Current Month Transactions
            var monthlyTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= startOfMonth)
                .ToListAsync();

            model.MonthlyIncome = monthlyTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            model.MonthlyExpense = monthlyTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            if (model.MonthlyIncome > 0)
            {
                var savings = model.MonthlyIncome - model.MonthlyExpense;
                model.SavingRate = savings > 0 ? (savings / model.MonthlyIncome) * 100 : 0;
            }

            // 4. Expense By Category Chart
            var expensesByCategory = monthlyTransactions
                .Where(t => t.Type == TransactionType.Expense && t.Category != null)
                .GroupBy(t => t.Category!.Name)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                .OrderByDescending(x => x.Amount)
                .ToList();

            model.ExpenseCategoryLabels = expensesByCategory.Select(x => x.Category).ToList();
            model.ExpenseCategoryData = expensesByCategory.Select(x => x.Amount).ToList();

            // 5. Cash Flow 6 Months
            for (int i = 5; i >= 0; i--)
            {
                var monthDate = today.AddMonths(-i);
                model.CashFlowLabels.Add(monthDate.ToString("MMM yy"));

                var start = new DateTime(monthDate.Year, monthDate.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);

                var monthData = await _context.Transactions
                    .Where(t => t.UserId == userId && t.Date >= start && t.Date <= end)
                    .ToListAsync();

                model.CashFlowIncomeData.Add(monthData.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount));
                model.CashFlowExpenseData.Add(monthData.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount));
            }

            // 6. Recent Transactions (Last 5)
            model.RecentTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.CreateAt) 
                .Take(5)
                .Select(t => new RecentTransactionViewModel
                {
                    Date = t.Date.ToString("dd/MM/yyyy"),
                    Description = string.IsNullOrEmpty(t.Description) ? (t.Category != null ? t.Category.Name : "Transfer") : t.Description,
                    CategoryName = t.Category != null ? t.Category.Name : "System",
                    Amount = t.Amount,
                    Type = t.Type.ToString()
                }).ToListAsync();

            return model;
        }
    }
}