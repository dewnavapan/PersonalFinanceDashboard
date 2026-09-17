using PersonalFinance.Core.Enums;
using System;
using System.Security.Principal;

namespace PersonalFinance.Core.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public Guid? CategoryId { get; set; } // Nullable สำหรับ Transfer
        public Guid? LinkedTransactionId { get; set; } // ผูก Transaction ขาเข้า-ขาออก

        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;

        // Navigation Properties (สร้างไว้ให้ EF Core รู้จักความสัมพันธ์)
        public User User { get; set; } = null!;
        public Account Account { get; set; } = null!;
        public Category? Category { get; set; }
        public Transaction? LinkedTransaction { get; set; }
    }
}