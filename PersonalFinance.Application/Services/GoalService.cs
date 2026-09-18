using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Goal;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context;

        public GoalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GoalProgressViewModel>> GetGoalsAsync(Guid userId)
        {
            var goals = await _context.FinancialGoals
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.TargetDate)
                .ToListAsync();

            return goals.Select(g => new GoalProgressViewModel
            {
                Id = g.Id,
                Name = g.Name,
                TargetAmount = g.TargetAmount,
                CurrentAmount = g.CurrentAmount,
                TargetDate = g.TargetDate
            });
        }

        public async Task CreateGoalAsync(GoalFormViewModel model, Guid userId)
        {
            var goal = new FinancialGoal
            {
                UserId = userId,
                Name = model.Name,
                TargetAmount = model.TargetAmount,
                CurrentAmount = model.CurrentAmount,
                TargetDate = model.TargetDate
            };

            _context.FinancialGoals.Add(goal);
            await _context.SaveChangesAsync();
        }

        public async Task AddFundToGoalAsync(Guid id, decimal amount, Guid userId)
        {
            var goal = await _context.FinancialGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (goal != null)
            {
                goal.CurrentAmount += amount;
                if (goal.CurrentAmount > goal.TargetAmount)
                    goal.CurrentAmount = goal.TargetAmount; // ป้องกันการออมเกินเป้าหมายจนตัวเลขเพี้ยน

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteGoalAsync(Guid id, Guid userId)
        {
            var goal = await _context.FinancialGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (goal != null)
            {
                _context.FinancialGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }
        }
    }
}