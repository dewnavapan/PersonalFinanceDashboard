namespace PersonalFinance.Application.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // Summary Cards
        public decimal TotalBalance { get; set; }
        public decimal NetWorth { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpense { get; set; }
        public decimal SavingRate { get; set; }

        // Chart: Expense By Category (Doughnut)
        public List<string> ExpenseCategoryLabels { get; set; } = new();
        public List<decimal> ExpenseCategoryData { get; set; } = new();

        // Chart: 6-Month Cash Flow (Bar/Line)
        public List<string> CashFlowLabels { get; set; } = new();
        public List<decimal> CashFlowIncomeData { get; set; } = new();
        public List<decimal> CashFlowExpenseData { get; set; } = new();

        // Recent Transactions
        public List<RecentTransactionViewModel> RecentTransactions { get; set; } = new();
    }

    public class RecentTransactionViewModel
    {
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}