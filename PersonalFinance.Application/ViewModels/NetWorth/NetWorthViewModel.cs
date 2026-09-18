namespace PersonalFinance.Application.ViewModels.NetWorth
{
    public class NetWorthViewModel
    {
        public decimal TotalAssets => CashAndBankAssets + InvestmentAssets;
        public decimal TotalLiabilities { get; set; }
        public decimal NetWorth => TotalAssets - TotalLiabilities;

        public decimal CashAndBankAssets { get; set; }
        public decimal InvestmentAssets { get; set; }

        public List<NetWorthItemViewModel> AssetDetails { get; set; } = new();
        public List<NetWorthItemViewModel> LiabilityDetails { get; set; } = new();
    }

    public class NetWorthItemViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}