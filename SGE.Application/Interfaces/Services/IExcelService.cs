namespace SGE.Application.Interfaces.Services;

public interface IExcelService
{
    Task<List<T>> ImportFromExcelAsync<T>(Stream fileStream, CancellationToken cancellationToken = default)
        where T : class, new();

    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName,
        CancellationToken cancellationToken = default) where T : class;
}