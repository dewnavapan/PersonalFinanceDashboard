using Microsoft.EntityFrameworkCore;
using PersonalFinance.Core.Entities;
using System.Reflection.Emit;
using System.Security.Principal;

namespace PersonalFinance.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // การตั้งค่า Self-referencing FK สำหรับ Transfer
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.LinkedTransaction)
                .WithMany()
                .HasForeignKey(t => t.LinkedTransactionId)
                .OnDelete(DeleteBehavior.Restrict); // ป้องกัน Cascade Delete วนลูป

            // กำหนด Precision ให้ตัวเลขการเงิน (สำคัญมาก ป้องกันเศษสตางค์เพี้ยน)
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 4);

            // สร้าง Index เพื่อให้ Dashboard โหลดเร็วขึ้น
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => new { t.UserId, t.Date });

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict); // ปิดการลบอัตโนมัติจาก User มาที่ Transaction

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}