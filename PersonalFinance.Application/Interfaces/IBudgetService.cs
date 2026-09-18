using PersonalFinance.Application.ViewModels.Budget;

namespace PersonalFinance.Application.Interfaces
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetProgressViewModel>> GetBudgetProgressAsync(Guid userId, int month, int year);
        Task CreateBudgetAsync(BudgetFormViewModel model, Guid userId);
        Task DeleteBudgetAsync(Guid id, Guid userId);
    }
}