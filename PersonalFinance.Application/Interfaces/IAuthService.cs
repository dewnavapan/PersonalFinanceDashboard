using System.Threading.Tasks;
using PersonalFinance.Core.Entities;
using PersonalFinance.Application.ViewModels.Auth;

namespace PersonalFinance.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(RegisterViewModel model);
        Task<bool> IsEmailExistAsync(string email);
    }
}