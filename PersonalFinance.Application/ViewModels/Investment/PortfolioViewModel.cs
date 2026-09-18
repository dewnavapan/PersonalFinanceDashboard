namespace PersonalFinance.Application.ViewModels.Investment
{
    public class PortfolioViewModel
    {
        public List<InvestmentItemViewModel> Assets { get; set; } = new();

        public decimal TotalInvested => Assets.Sum(a => a.InvestedAmount);
        public decimal TotalCurrentValue => Assets.Sum(a => a.CurrentValue);
        public decimal TotalProfitLoss => TotalCurrentValue - TotalInvested;
        public decimal TotalReturnPercentage => TotalInvested > 0 ? (TotalProfitLoss / TotalInvested) * 100 : 0;
    }

    public class InvestmentItemViewModel
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal CurrentPrice { get; set; }

        public decimal InvestedAmount => TotalQuantity * AverageCost;
        public decimal CurrentValue => TotalQuantity * CurrentPrice;
        public decimal ProfitLoss => CurrentValue - InvestedAmount;
        public decimal ReturnPercentage => InvestedAmount > 0 ? (ProfitLoss / InvestedAmount) * 100 : 0;

        public string StatusColor => ProfitLoss >= 0 ? "text-success" : "text-danger";
    }
}