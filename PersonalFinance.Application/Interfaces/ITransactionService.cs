using PersonalFinance.Application.ViewModels.Transaction;

namespace PersonalFinance.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<bool> CreateTransactionAsync(TransactionFormViewModel model, Guid userId);
    }
}