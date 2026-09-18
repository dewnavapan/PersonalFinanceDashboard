using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.ViewModels.Investment
{
    public class InvestmentFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "กรุณาระบุตัวย่อสินทรัพย์")]
        public string Symbol { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        [Required]
        public string AssetType { get; set; } = "Stock";

        [Required]
        [Range(0.000001, double.MaxValue, ErrorMessage = "จำนวนต้องมากกว่า 0")]
        public decimal TotalQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "ต้นทุนต้องมากกว่า 0")]
        public decimal AverageCost { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "ราคาปัจจุบันต้องมากกว่า 0")]
        public decimal CurrentPrice { get; set; }
    }
}