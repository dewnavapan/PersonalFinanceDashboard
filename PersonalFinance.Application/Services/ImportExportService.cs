using System.Text;
//using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Core.Entities;
using PersonalFinance.Core.Enums;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class ImportExportService : IImportExportService
    {
        private readonly ApplicationDbContext _context;

        public ImportExportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportTransactionsToCsvAsync(Guid userId)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Date)
                .ToListAsync();

            var builder = new StringBuilder();
            // Header
            builder.AppendLine("Date,Type,Category,Amount,Account,Description");

            foreach (var t in transactions)
            {
                var date = t.Date.ToString("yyyy-MM-dd");
                var type = t.Type.ToString();
                var category = t.Category != null ? t.Category.Name : "";
                var account = t.Account != null ? t.Account.Name : "";
                // Escape commas in description
                var desc = $"\"{t.Description?.Replace("\"", "\"\"")}\"";

                builder.AppendLine($"{date},{type},{category},{t.Amount},{account},{desc}");
            }

            // ใช้ UTF8 แบบมี BOM เพื่อให้เปิดใน Excel ภาษาไทยได้ไม่เพี้ยน
            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(builder.ToString())).ToArray();
        }

        public async Task<(int successCount, List<string> errors)> ImportTransactionsFromCsvAsync(Stream fileStream, Guid userId)
        {
            var errors = new List<string>();
            int successCount = 0;

      
            using var reader = new StreamReader(fileStream);
            var header = await reader.ReadLineAsync();

            var accounts = await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();
            var categories = await _context.Categories.Where(c => c.UserId == userId).ToListAsync();

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int row = 2;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',');

                    if (values.Length < 5)
                    {
                        errors.Add($"Row {row}: ข้อมูลไม่ครบถ้วน");
                        row++;
                        continue;
                    }

                    // Parse ข้อมูล
                    if (!DateTime.TryParse(values[0], out var date))
                    {
                        errors.Add($"Row {row}: รูปแบบวันที่ไม่ถูกต้อง ({values[0]})");
                        row++; continue;
                    }

                    if (!Enum.TryParse<TransactionType>(values[1], true, out var type))
                    {
                        errors.Add($"Row {row}: ประเภทธุรกรรมไม่ถูกต้อง ({values[1]})");
                        row++; continue;
                    }

                    var categoryName = values[2].Trim();
                    var category = categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                    if (category == null && type != TransactionType.TransferIn && type != TransactionType.TransferOut)
                    {
                        errors.Add($"Row {row}: ไม่พบหมวดหมู่ '{categoryName}' ในระบบ");
                        row++; continue;
                    }

                    if (!decimal.TryParse(values[3], out var amount) || amount <= 0)
                    {
                        errors.Add($"Row {row}: จำนวนเงินไม่ถูกต้อง ({values[3]})");
                        row++; continue;
                    }

                    var accountName = values[4].Trim();
                    var account = accounts.FirstOrDefault(a => a.Name.Equals(accountName, StringComparison.OrdinalIgnoreCase));
                    if (account == null)
                    {
                        errors.Add($"Row {row}: ไม่พบบัญชี '{accountName}' ในระบบ");
                        row++; continue;
                    }

                    var description = values.Length > 5 ? values[5].Trim('"') : "";

                    // สร้าง Transaction และอัปเดต Balance (รองรับเฉพาะ Income/Expense เบื้องต้น เพื่อความปลอดภัยของข้อมูล)
                    if (type == TransactionType.Income || type == TransactionType.Expense)
                    {
                        var transaction = new Transaction
                        {
                            UserId = userId,
                            AccountId = account.Id,
                            CategoryId = category?.Id,
                            Type = type,
                            Amount = amount,
                            Date = date,
                            Description = description
                        };

                        if (type == TransactionType.Income) account.Balance += amount;
                        else account.Balance -= amount;

                        await _context.Transactions.AddAsync(transaction);
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"Row {row}: ระบบ Import ยังไม่รองรับประเภท Transfer");
                    }
                    row++;
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("เกิดข้อผิดพลาดร้ายแรง ระบบได้ยกเลิกการนำเข้าข้อมูลทั้งหมด");
            }

            return (successCount, errors);
        }
    }
}