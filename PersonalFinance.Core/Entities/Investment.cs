using System.ComponentModel.DataAnnotations;
using PersonalFinance.Core.Enums;

namespace PersonalFinance.Core.Entities
{
    public class Investment : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Symbol { get; set; } = string.Empty;

        public AssetType AssetType { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal AverageCost { get; set; }

        public decimal CurrentPrice { get; set; }

        public User User { get; set; } = null!;
        public ICollection<InvestmentTransaction> Transactions { get; set; } = new List<InvestmentTransaction>();

        public string Name { get; set; } = string.Empty;
    }
}