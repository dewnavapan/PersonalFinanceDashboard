namespace PersonalFinance.Application.ViewModels.Budget
{
    public class BudgetProgressViewModel
    {
        public Guid BudgetId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public decimal UsedAmount { get; set; }

        public decimal RemainingAmount => BudgetAmount - UsedAmount;
        public decimal UsagePercentage => BudgetAmount > 0 ? (UsedAmount / BudgetAmount) * 100 : 0;

        public string StatusColor
        {
            get
            {
                if (UsagePercentage >= 100) return "bg-danger"; // Over Budget
                if (UsagePercentage >= 80) return "bg-warning"; // Near Limit
                return "bg-success"; // Under Budget
            }
        }

        public string StatusText
        {
            get
            {
                if (UsagePercentage >= 100) return "เกินงบประมาณ";
                if (UsagePercentage >= 80) return "ใกล้ถึงขีดจำกัด";
                return "อยู่ในเกณฑ์ปกติ";
            }
        }
    }
}