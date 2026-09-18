using System.IO;

namespace PersonalFinance.Application.Interfaces
{
    public interface IImportExportService
    {
        Task<byte[]> ExportTransactionsToCsvAsync(Guid userId);

        Task<(int successCount, List<string> errors)> ImportTransactionsFromCsvAsync(Stream fileStream, Guid userId);
    }
}