using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Portfolio;
using PersonalFinance.Core.Enums;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly ApplicationDbContext _context;

        public PortfolioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PortfolioSummaryViewModel> GetPortfolioSummaryAsync(Guid userId)
        {
            // ดึงข้อมูลสินทรัพย์พร้อมประวัติการซื้อขายทั้งหมดของ User
            var investments = await _context.Investments
                .Include(i => i.Transactions)
                .Where(i => i.UserId == userId)
                .ToListAsync();

            var summary = new PortfolioSummaryViewModel();

            foreach (var inv in investments)
            {
                decimal totalQty = 0;
                decimal totalCost = 0;

                // คำนวณแบบง่าย: ต้นทุนรวม (Buy) หักลบด้วยต้นทุนที่ขายออกไป (Sell)
                foreach (var tx in inv.Transactions.OrderBy(t => t.Date))
                {
                    if (tx.Type == InvestmentTransactionType.Buy)
                    {
                        totalQty += tx.Quantity;
                        totalCost += (tx.Quantity * tx.Price) + tx.Fee;
                    }
                    else if (tx.Type == InvestmentTransactionType.Sell)
                    {
                        // การคำนวณแบบ Average Cost
                        if (totalQty > 0)
                        {
                            decimal avgCostPerUnit = totalCost / totalQty;
                            totalCost -= (tx.Quantity * avgCostPerUnit);
                            totalQty -= tx.Quantity;
                        }
                    }
                }

                // ข้ามสินทรัพย์ที่ขายไปหมดแล้ว (Quantity <= 0)
                if (totalQty <= 0) continue;

                var assetVm = new InvestmentItemViewModel
                {
                    InvestmentId = inv.Id,
                    Symbol = inv.Symbol,
                    Name = inv.Name,
                    AssetType = inv.AssetType.ToString(),
                    TotalQuantity = totalQty,
                    AverageCost = totalCost / totalQty,
                    CurrentPrice = inv.CurrentPrice
                };

                summary.Assets.Add(assetVm);

                // บวกตัวเลขเข้ายอดรวมพอร์ต
                summary.TotalInvested += assetVm.TotalInvested;
                summary.TotalMarketValue += assetVm.MarketValue;
            }

            return summary;
        }
        // เพิ่มฟังก์ชันนี้ต่อจาก GetPortfolioSummaryAsync ในคลาส PortfolioService
        public async Task<bool> AddTransactionAsync(InvestmentTransactionFormViewModel model, Guid userId)
        {
            // 1. ค้นหาว่าเคยมีสินทรัพย์ Symbol นี้ในพอร์ตของ User หรือยัง (แปลงเป็นตัวพิมพ์ใหญ่เพื่อป้องกันการซ้ำซ้อน)
            string symbolUpper = model.Symbol.ToUpper().Trim();
            var investment = await _context.Investments
                .FirstOrDefaultAsync(i => i.Symbol == symbolUpper && i.UserId == userId);

            // 2. ถ้ายังไม่มี ให้สร้างสินทรัพย์ใหม่
            if (investment == null)
            {
                investment = new PersonalFinance.Core.Entities.Investment
                {
                    UserId = userId,
                    Symbol = symbolUpper,
                    Name = string.IsNullOrWhiteSpace(model.Name) ? symbolUpper : model.Name,
                    AssetType = model.AssetType,
                    CurrentPrice = model.Price // ตั้งราคาปัจจุบันให้เท่ากับราคาที่ซื้อครั้งแรก
                };
                await _context.Investments.AddAsync(investment);
            }
            else
            {
                // ถ้ามีอยู่แล้วและเป็นการซื้อ หรือมีราคาใหม่ ให้อัปเดต CurrentPrice (แบบง่าย)
                investment.CurrentPrice = model.Price;
            }

            // 3. สร้าง Transaction การซื้อขาย
            var transaction = new PersonalFinance.Core.Entities.InvestmentTransaction
            {
                Investment = investment, // ผูกกับสินทรัพย์
                Type = model.Type,
                Quantity = model.Quantity,
                Price = model.Price,
                Fee = model.Fee,
                Date = model.Date
            };

            await _context.Set<PersonalFinance.Core.Entities.InvestmentTransaction>().AddAsync(transaction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}