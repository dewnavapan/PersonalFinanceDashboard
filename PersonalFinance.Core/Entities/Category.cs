using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Core.Entities
{
    public class Category : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty; // Income หรือ Expense

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(20)]
        public string? Color { get; set; }

        public User User { get; set; } = null!;
    }
}