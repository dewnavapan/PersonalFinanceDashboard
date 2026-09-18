using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Investment;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly ApplicationDbContext _context;

        public InvestmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PortfolioViewModel> GetPortfolioAsync(Guid userId)
        {
            var investments = await _context.Investments
                .Where(i => i.UserId == userId)
                .ToListAsync();

            var portfolio = new PortfolioViewModel
            {
                Assets = investments.Select(i => new InvestmentItemViewModel
                {
                    Id = i.Id,
                    Symbol = i.Symbol,
                    AssetType = i.AssetType.ToString(),
                    TotalQuantity = i.TotalQuantity,
                    AverageCost = i.AverageCost,
                    CurrentPrice = i.CurrentPrice
                }).OrderByDescending(a => a.CurrentValue).ToList()
            };

            return portfolio;
        }

        public async Task SaveInvestmentAsync(InvestmentFormViewModel model, Guid userId)
        {
            if (model.Id.HasValue && model.Id.Value != Guid.Empty)
            {
                var existing = await _context.Investments.FirstOrDefaultAsync(i => i.Id == model.Id && i.UserId == userId);
                if (existing != null)
                {
                    existing.TotalQuantity = model.TotalQuantity;
                    existing.AverageCost = model.AverageCost;
                    existing.CurrentPrice = model.CurrentPrice;
                }
            }
            else
            {
                var investment = new Investment
                {
                    UserId = userId,
                    Symbol = model.Symbol.ToUpper(),
                    //   Name = model.Symbol,
                    AssetType = Enum.Parse<PersonalFinance.Core.Enums.AssetType>(model.AssetType),
                    TotalQuantity = model.TotalQuantity,
                    AverageCost = model.AverageCost,
                    CurrentPrice = model.CurrentPrice
                };
                _context.Investments.Add(investment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMarketPriceAsync(Guid id, decimal newPrice, Guid userId)
        {
            var investment = await _context.Investments.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (investment != null)
            {
                investment.CurrentPrice = newPrice;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteInvestmentAsync(Guid id, Guid userId)
        {
            var investment = await _context.Investments.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (investment != null)
            {
                _context.Investments.Remove(investment);
                await _context.SaveChangesAsync();
            }
        }
    }
}