using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Core.Entities
{
    public class FinancialGoal : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        public DateTime TargetDate { get; set; }

        public User User { get; set; } = null!;
    }
}