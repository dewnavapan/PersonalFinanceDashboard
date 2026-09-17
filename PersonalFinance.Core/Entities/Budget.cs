using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Core.Entities
{
    public class Budget : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}