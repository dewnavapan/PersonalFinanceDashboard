using PersonalFinance.Application.ViewModels.NetWorth;

namespace PersonalFinance.Application.Interfaces
{
    public interface INetWorthService
    {
        Task<NetWorthViewModel> GetNetWorthSummaryAsync(Guid userId);
    }
}