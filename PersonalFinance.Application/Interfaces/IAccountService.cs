using PersonalFinance.Core.Entities;

namespace PersonalFinance.Application.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId);
    }
}