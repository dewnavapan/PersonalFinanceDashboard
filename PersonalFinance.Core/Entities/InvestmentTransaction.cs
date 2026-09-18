using PersonalFinance.Core.Enums;

namespace PersonalFinance.Core.Entities
{
    public class InvestmentTransaction : BaseEntity
    {
        public Guid InvestmentId { get; set; }

        public InvestmentTransactionType Type { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Fee { get; set; }

        public DateTime TransactionDate { get; set; }

        public Investment Investment { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}