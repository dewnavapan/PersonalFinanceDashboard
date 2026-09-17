using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinance.Application.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // Financial Summary
        public decimal TotalBalance { get; set; }
        public decimal NetWorth { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal SavingRate { get; set; }
        public decimal NetCashFlow => TotalIncome - TotalExpense;

        // Smart Insights
        public List<string> SmartInsights { get; set; } = new List<string>();

        // Chart Data (แปลงเป็น JSON string เพื่อส่งให้ Chart.js ในหน้า View)
        public string ExpenseCategoryLabelsJson { get; set; } = "[]";
        public string ExpenseCategoryDataJson { get; set; } = "[]";
    }
}
