namespace PersonalFinance.Application.ViewModels.Goal
{
    public class GoalProgressViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime TargetDate { get; set; }

        // คำนวณเปอร์เซ็นต์ความสำเร็จ
        public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;

        // คำนวณจำนวนเดือนที่เหลือ
        public int MonthsRemaining
        {
            get
            {
                var today = DateTime.Today;
                var months = ((TargetDate.Year - today.Year) * 12) + TargetDate.Month - today.Month;
                return months > 0 ? months : 0;
            }
        }

        // คำนวณยอดที่ต้องออมต่อเดือน
        public decimal RequiredMonthlySaving
        {
            get
            {
                if (CurrentAmount >= TargetAmount) return 0; // บรรลุเป้าหมายแล้ว
                if (MonthsRemaining <= 0) return TargetAmount - CurrentAmount; // หมดเวลา ต้องจ่ายก้อนสุดท้าย
                return (TargetAmount - CurrentAmount) / MonthsRemaining;
            }
        }
    }
}