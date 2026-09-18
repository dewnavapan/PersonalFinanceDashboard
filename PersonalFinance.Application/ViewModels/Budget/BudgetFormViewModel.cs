using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.ViewModels.Budget
{
    public class BudgetFormViewModel
    {
        [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "กรุณาระบุจำนวนเงิน")]
        [Range(1, double.MaxValue, ErrorMessage = "งบประมาณต้องมากกว่า 0")]
        public decimal Amount { get; set; }

        public int Month { get; set; } = DateTime.Today.Month;
        public int Year { get; set; } = DateTime.Today.Year;
    }
}