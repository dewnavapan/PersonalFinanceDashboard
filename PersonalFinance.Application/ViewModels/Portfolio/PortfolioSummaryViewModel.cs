namespace PersonalFinance.Application.ViewModels.Portfolio
{
    public class PortfolioSummaryViewModel
    {
        public decimal TotalInvested { get; set; }
        public decimal TotalMarketValue { get; set; }
        public decimal TotalUnrealizedProfitLoss => TotalMarketValue - TotalInvested;
        public decimal TotalRoiPercentage => TotalInvested > 0 ? (TotalUnrealizedProfitLoss / TotalInvested) * 100 : 0;

        public List<InvestmentItemViewModel> Assets { get; set; } = new();
    }

    public class InvestmentItemViewModel
    {
        public Guid InvestmentId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;

        public decimal TotalQuantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal CurrentPrice { get; set; }

        public decimal TotalInvested => TotalQuantity * AverageCost;
        public decimal MarketValue => TotalQuantity * CurrentPrice;
        public decimal UnrealizedProfitLoss => MarketValue - TotalInvested;
        public decimal RoiPercentage => TotalInvested > 0 ? (UnrealizedProfitLoss / TotalInvested) * 100 : 0;
    }
}