using System.Collections.Generic;

namespace PersonalFinance.Application.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // สรุปยอดรวม (ภาพรวม)
        public decimal TotalBalance { get; set; }

        // สรุปประจำเดือนปัจจุบัน
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpense { get; set; }
        public decimal MonthlySaving => MonthlyIncome - MonthlyExpense;

        // ข้อมูลสำหรับกราฟ (รายจ่ายแยกตามหมวดหมู่)
        public Dictionary<string, decimal> ExpenseByCategory { get; set; } = new Dictionary<string, decimal>();
    }
}