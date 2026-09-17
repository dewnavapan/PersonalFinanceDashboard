using System.ComponentModel.DataAnnotations;
using PersonalFinance.Core.Enums;

namespace PersonalFinance.Application.ViewModels.Transaction
{
    public class TransactionFormViewModel
    {
        [Required(ErrorMessage = "กรุณาระบุบัญชี")]
        public Guid AccountId { get; set; }

        public Guid? ToAccountId { get; set; } // ใช้สำหรับ Transfer เท่านั้น

        public Guid? CategoryId { get; set; } // ใช้สำหรับ Income/Expense

        [Required]
        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "กรุณาระบุจำนวนเงิน")]
        [Range(0.01, double.MaxValue, ErrorMessage = "จำนวนเงินต้องมากกว่า 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;
    }
}