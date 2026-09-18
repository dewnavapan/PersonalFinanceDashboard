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
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Investment> Investments { get; set; }


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

            modelBuilder.Entity<Budget>()
        .Property(b => b.Amount)
        .HasPrecision(18, 4);

            // 2. ปิด Cascade Delete เพื่อแก้ Multiple Cascade Paths
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Investment>()
        .Property(i => i.TotalQuantity).HasPrecision(18, 6);
            modelBuilder.Entity<Investment>()
                .Property(i => i.AverageCost).HasPrecision(18, 6);
            modelBuilder.Entity<Investment>()
                .Property(i => i.CurrentPrice).HasPrecision(18, 6);

            modelBuilder.Entity<InvestmentTransaction>()
                .Property(it => it.Quantity).HasPrecision(18, 6);
            modelBuilder.Entity<InvestmentTransaction>()
                .Property(it => it.Price).HasPrecision(18, 6);
            modelBuilder.Entity<InvestmentTransaction>()
                .Property(it => it.Fee).HasPrecision(18, 4);

            // ตั้งค่า Cascade Delete 
            modelBuilder.Entity<InvestmentTransaction>()
                .HasOne(it => it.Investment)
                .WithMany(i => i.Transactions)
                .HasForeignKey(it => it.InvestmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Investment>()
        .Property(i => i.AssetType)
        .HasConversion<string>();
        }
    }
}