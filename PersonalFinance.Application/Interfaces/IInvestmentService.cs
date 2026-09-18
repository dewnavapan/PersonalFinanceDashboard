using PersonalFinance.Application.ViewModels.Investment;

namespace PersonalFinance.Application.Interfaces
{
    public interface IInvestmentService
    {
        Task<PortfolioViewModel> GetPortfolioAsync(Guid userId);
        Task SaveInvestmentAsync(InvestmentFormViewModel model, Guid userId);
        Task UpdateMarketPriceAsync(Guid id, decimal newPrice, Guid userId);
        Task DeleteInvestmentAsync(Guid id, Guid userId);
    }
}