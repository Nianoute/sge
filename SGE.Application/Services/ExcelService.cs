using System.ComponentModel;
using OfficeOpenXml;
using SGE.Application.Interfaces.Services;

namespace SGE.Application.Services;

public class ExcelService : IExcelService
{
    public ExcelService()
    {
        // Configuration de la licence EPPlus (NonCommercial pour usage gratuit)
        ExcelPackage.License.SetNonCommercialPersonal("Enzo Angot ");
    }

    public async Task<List<T>> ImportFromExcelAsync<T>(Stream fileStream, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var result = new List<T>();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0]; // On part du principe qu'il n'y a qu'une feuille de donnée
        var rowCount = worksheet.Dimension?.Rows ?? 0;
        var colCount = worksheet.Dimension?.Columns ?? 0;

        if (rowCount < 2) // Si pas de donnée alors on retourne le résultat
            return result;

        var properties = typeof(T).GetProperties();
        var headers = new Dictionary<int, string>();

        // Lecture des entêtes 
        for (var col = 1; col <= colCount; col++)
        {
            var header = worksheet.Cells[1, col].Text.Trim();
            headers[col] = header;
        }

        // Lecture des données à partir de la ligne 2 (car ligne 1 = entête)
        for (var row = 2; row <= rowCount; row++)
        {
            var item = new T();
            var itemType = typeof(T);

            foreach (var kvp in headers)
            {
                var col = kvp.Key;
                var headerName = kvp.Value;

                // Trouver la propriété correspondante (insensible à la casse)
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase));

                if (property == null) continue;

                var cellValue = worksheet.Cells[row, col].Text;

                try
                {
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        var convertedValue = ConvertValue(cellValue, property.PropertyType);
                        property.SetValue(item, convertedValue);
                    }
                }
                catch
                {
                    // Ignorer les erreurs de conversion ici, la validation les attrapera
                }
            }

            // On ajoute le numéro de ligne pour le tracking des erreurs
            var rowNumberProperty = itemType.GetProperty("RowNumber");
            rowNumberProperty?.SetValue(item, row);

            result.Add(item);
        }

        return result;
    }

    public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName,
        CancellationToken cancellationToken = default) where T : class
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        var properties = typeof(T).GetProperties()
            .Where(p => p.Name != "Id") // L'id ne doit pas apparaître dans l'export
            .ToList();

        // En-têtes de l'export
        for (var i = 0; i < properties.Count; i++)
        {
            worksheet.Cells[1, i + 1].Value = properties[i].Name;
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        // Données à ajouter dans l'export
        var dataList = data.ToList();
        for (var row = 0; row < dataList.Count; row++)
        for (var col = 0; col < properties.Count; col++)
        {
            var value = properties[col].GetValue(dataList[row]);
            worksheet.Cells[row + 2, col + 1].Value = value;

            // Formatage spécial pour les dates
            if (value is DateTime) worksheet.Cells[row + 2, col + 1].Style.Numberformat.Format = "dd/mm/yyyy";
        }

        // Auto-fit des colonnes ( Pas obligatoire mais utile pour avoir un fichier excel propre )
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return await Task.FromResult(package.GetAsByteArray());
    }

    private object? ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Gérer les types nullable
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(DateTime))
            if (DateTime.TryParse(value, out var dateResult))
                return DateTime.SpecifyKind(dateResult, DateTimeKind.Utc);

        if (underlyingType == typeof(decimal))
            if (decimal.TryParse(value, out var decimalResult))
                return decimalResult;

        if (underlyingType == typeof(int))
            if (int.TryParse(value, out var intResult))
                return intResult;

        var converter = TypeDescriptor.GetConverter(underlyingType);
        if (converter.CanConvertFrom(typeof(string))) return converter.ConvertFromString(value);

        return value;
    }
}