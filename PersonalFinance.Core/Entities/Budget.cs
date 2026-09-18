using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Core.Entities
{
    public class Budget : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }

        public decimal Amount { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}