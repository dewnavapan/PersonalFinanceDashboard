using PersonalFinance.Application.ViewModels.Portfolio;

namespace PersonalFinance.Application.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioSummaryViewModel> GetPortfolioSummaryAsync(Guid userId);
        // เพิ่มฟังก์ชันนี้
        Task<bool> AddTransactionAsync(InvestmentTransactionFormViewModel model, Guid userId);
    }
}