using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.NetWorth;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class NetWorthService : INetWorthService
    {
        private readonly ApplicationDbContext _context;

        public NetWorthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NetWorthViewModel> GetNetWorthSummaryAsync(Guid userId)
        {
            var model = new NetWorthViewModel();

            // 1. ดึงข้อมูลบัญชีทั้งหมด (Cash, Bank, E-Wallet)
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();

            // แยกสินทรัพย์ (เงินบวก) และ หนี้สิน (เงินติดลบ)
            foreach (var acc in accounts)
            {
                if (acc.Balance >= 0)
                {
                    model.CashAndBankAssets += acc.Balance;
                    model.AssetDetails.Add(new NetWorthItemViewModel
                    {
                        Name = acc.Name,
                        Category = $"บัญชี ({acc.Type})",
                        Amount = acc.Balance
                    });
                }
                else
                {
                    // เปลี่ยนค่าติดลบให้เป็นบวกสำหรับการแสดงผลหนี้สิน
                    var liabilityAmount = Math.Abs(acc.Balance);
                    model.TotalLiabilities += liabilityAmount;
                    model.LiabilityDetails.Add(new NetWorthItemViewModel
                    {
                        Name = acc.Name,
                        Category = $"หนี้สิน ({acc.Type})",
                        Amount = liabilityAmount
                    });
                }
            }

            // 2. ดึงข้อมูลการลงทุนทั้งหมด
            var investments = await _context.Investments
                .Where(i => i.UserId == userId)
                .ToListAsync();

            foreach (var inv in investments)
            {
                var currentValue = inv.TotalQuantity * inv.CurrentPrice;
                model.InvestmentAssets += currentValue;

                model.AssetDetails.Add(new NetWorthItemViewModel
                {
                    Name = $"{inv.Symbol} ({inv.Symbol})",
                    Category = $"การลงทุน ({inv.AssetType})",
                    Amount = currentValue
                });
            }

            return model;
        }
    }
}