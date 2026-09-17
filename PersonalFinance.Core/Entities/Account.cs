using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Core.Entities
{
    public class Account : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Cash"; // Cash, Bank, E-Wallet, Investment

        public decimal Balance { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "THB";

        public User User { get; set; } = null!;
    }
}