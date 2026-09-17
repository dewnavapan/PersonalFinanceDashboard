using PersonalFinance.Application.ViewModels.Dashboard;

namespace PersonalFinance.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardSummaryAsync(Guid userId, int month, int year);
    }
}