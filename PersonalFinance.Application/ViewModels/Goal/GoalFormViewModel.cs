using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.ViewModels.Goal
{
    public class GoalFormViewModel
    {
        [Required(ErrorMessage = "กรุณาระบุชื่อเป้าหมาย")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาระบุยอดเงินเป้าหมาย")]
        [Range(1, double.MaxValue, ErrorMessage = "ยอดเป้าหมายต้องมากกว่า 0")]
        public decimal TargetAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "ยอดเงินปัจจุบันต้องไม่ติดลบ")]
        public decimal CurrentAmount { get; set; }

        [Required(ErrorMessage = "กรุณาระบุวันที่เป้าหมาย")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddYears(1);
    }
}