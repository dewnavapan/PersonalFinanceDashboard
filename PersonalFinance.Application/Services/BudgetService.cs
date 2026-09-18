using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Budget;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;

        public BudgetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BudgetProgressViewModel>> GetBudgetProgressAsync(Guid userId, int month, int year)
        {
            // ดึงงบประมาณของเดือน/ปี ที่ระบุ
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                .ToListAsync();

            // ดึงรายจ่ายของเดือน/ปี ที่ระบุ เพื่อมาหักลบ
            var expenses = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date.Month == month && t.Date.Year == year && t.Type == Core.Enums.TransactionType.Expense)
                .GroupBy(t => t.CategoryId)
                .Select(g => new { CategoryId = g.Key, TotalUsed = g.Sum(t => t.Amount) })
                .ToDictionaryAsync(x => x.CategoryId, x => x.TotalUsed);

            var progressList = new List<BudgetProgressViewModel>();

            foreach (var budget in budgets)
            {
                decimal used = expenses.ContainsKey(budget.CategoryId) ? expenses[budget.CategoryId] : 0;

                progressList.Add(new BudgetProgressViewModel
                {
                    BudgetId = budget.Id,
                    CategoryName = budget.Category.Name,
                    BudgetAmount = budget.Amount,
                    UsedAmount = used
                });
            }

            return progressList.OrderByDescending(p => p.UsagePercentage);
        }

        public async Task CreateBudgetAsync(BudgetFormViewModel model, Guid userId)
        {
            // เช็คว่าเคยตั้งงบหมวดหมู่นี้ในเดือนนี้หรือยัง ถ้ามีให้อัปเดต ถ้าไม่มีให้สร้างใหม่
            var existingBudget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == model.CategoryId && b.Month == model.Month && b.Year == model.Year);

            if (existingBudget != null)
            {
                existingBudget.Amount = model.Amount;
                _context.Budgets.Update(existingBudget);
            }
            else
            {
                var budget = new Budget
                {
                    UserId = userId,
                    CategoryId = model.CategoryId,
                    Amount = model.Amount,
                    Month = model.Month,
                    Year = model.Year
                };
                _context.Budgets.Add(budget);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(Guid id, Guid userId)
        {
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
        }
    }
}