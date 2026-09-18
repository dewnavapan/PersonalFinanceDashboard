using PersonalFinance.Application.ViewModels.Goal;

namespace PersonalFinance.Application.Interfaces
{
    public interface IGoalService
    {
        Task<IEnumerable<GoalProgressViewModel>> GetGoalsAsync(Guid userId);
        Task CreateGoalAsync(GoalFormViewModel model, Guid userId);
        Task AddFundToGoalAsync(Guid id, decimal amount, Guid userId);
        Task DeleteGoalAsync(Guid id, Guid userId);
    }
}