using PersonalFinance.Core.Entities;

namespace PersonalFinance.Application.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId);

        Task CreateAccountAsync(Account account);

        Task<Account?> GetAccountByIdAsync(Guid id, Guid userId);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(Guid id, Guid userId);
    }
}