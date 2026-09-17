using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Transaction;
using PersonalFinance.Core.Enums;
using PersonalFinance.Infrastructure.Data;
using EntityTransaction = PersonalFinance.Core.Entities.Transaction; // ป้องกันชื่อซ้ำ

namespace PersonalFinance.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateTransactionAsync(TransactionFormViewModel model, Guid userId)
        {
            // เริ่มต้น Database Transaction (เผื่อระบบล่มระหว่างโอนเงิน)
            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.AccountId && a.UserId == userId);
                if (account == null) throw new Exception("ไม่พบบัญชีต้นทาง");

                if (model.Type == TransactionType.Income || model.Type == TransactionType.Expense)
                {
                    // 1. จัดการ รายรับ / รายจ่าย ปกติ
                    var transaction = new EntityTransaction
                    {
                        UserId = userId,
                        AccountId = model.AccountId,
                        CategoryId = model.CategoryId,
                        Type = model.Type,
                        Amount = model.Amount,
                        Date = model.Date,
                        Description = model.Description
                    };

                    // อัปเดตยอดเงินในบัญชี
                    if (model.Type == TransactionType.Income)
                        account.Balance += model.Amount;
                    else
                        account.Balance -= model.Amount;

                    await _context.Transactions.AddAsync(transaction);
                }
                else if (model.Type == TransactionType.TransferOut)
                {
                    // 2. จัดการ โอนเงิน (Double-Entry Lite)
                    if (model.ToAccountId == null) throw new Exception("กรุณาระบุบัญชีปลายทาง");
                    if (model.AccountId == model.ToAccountId) throw new Exception("ไม่สามารถโอนเงินเข้าบัญชีเดียวกันได้");

                    var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.ToAccountId && a.UserId == userId);
                    if (toAccount == null) throw new Exception("ไม่พบบัญชีปลายทาง");

                    // สร้างฝั่งโอนออก
                    var transferOut = new EntityTransaction
                    {
                        UserId = userId,
                        AccountId = model.AccountId,
                        Type = TransactionType.TransferOut,
                        Amount = model.Amount, // ค่าบวก (ตอน Query สรุปยอดค่อยไปหักลบ)
                        Date = model.Date,
                        Description = model.Description ?? $"โอนเงินไป {toAccount.Name}"
                    };

                    // สร้างฝั่งรับเข้า
                    var transferIn = new EntityTransaction
                    {
                        UserId = userId,
                        AccountId = model.ToAccountId.Value,
                        Type = TransactionType.TransferIn,
                        Amount = model.Amount,
                        Date = model.Date,
                        Description = model.Description ?? $"รับเงินโอนจาก {account.Name}"
                    };

                    // ผูกทั้งสองรายการเข้าด้วยกัน
                    transferOut.LinkedTransaction = transferIn;
                    transferIn.LinkedTransaction = transferOut;

                    // หักเงินต้นทาง เพิ่มเงินปลายทาง
                    account.Balance -= model.Amount;
                    toAccount.Balance += model.Amount;

                    await _context.Transactions.AddRangeAsync(transferOut, transferIn);
                }

                // บันทึกลง Database
                await _context.SaveChangesAsync();

                // ถ้ายืนยันสำเร็จ ให้ Commit (เสร็จสิ้นการทำงาน)
                await dbTransaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // ถ้ามีอะไรพัง ให้ Rollback ยกเลิกทุกอย่าง
                await dbTransaction.RollbackAsync();
                // (ใน Production ควรมี _logger.LogError(ex, "..."); ตรงนี้)
                throw;
            }
        }
    }
}