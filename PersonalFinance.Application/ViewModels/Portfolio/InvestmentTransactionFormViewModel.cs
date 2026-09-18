using System.ComponentModel.DataAnnotations;
using PersonalFinance.Core.Enums;

namespace PersonalFinance.Application.ViewModels.Portfolio
{
    public class InvestmentTransactionFormViewModel
    {
        [Required(ErrorMessage = "กรุณาระบุสัญลักษณ์ เช่น PTT, AAPL, BTC")]
        [MaxLength(20)]
        public string Symbol { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาระบุประเภทสินทรัพย์")]
        public AssetType AssetType { get; set; } // เช่น Stock, Crypto, MutualFund

        [Required(ErrorMessage = "กรุณาระบุประเภทรายการ")]
        public InvestmentTransactionType Type { get; set; } // Buy, Sell

        [Required(ErrorMessage = "กรุณาระบุจำนวน")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "จำนวนต้องมากกว่า 0")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "กรุณาระบุราคาต่อหน่วย")]
        [Range(0.01, double.MaxValue, ErrorMessage = "ราคาต้องมากกว่า 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "ค่าธรรมเนียมห้ามติดลบ")]
        public decimal Fee { get; set; } = 0;

        [Required(ErrorMessage = "กรุณาระบุวันที่ทำรายการ")]
        public DateTime Date { get; set; } = DateTime.Today;
    }
}