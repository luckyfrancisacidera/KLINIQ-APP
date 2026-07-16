using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Kliniq.SymptomCatalogBuilder;

internal static class CsvDatasetReader
{
    public static IEnumerable<IReadOnlyDictionary<string, string>> ReadRows(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim,
            DetectDelimiter = true
        });

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord ?? [];
        while (csv.Read())
        {
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers)
                row[header] = csv.GetField(header) ?? string.Empty;
            yield return row;
        }
    }

    public static string Get(IReadOnlyDictionary<string, string> row, params string[] names)
    {
        foreach (var name in names)
            if (row.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value)) return value;
        return string.Empty;
    }
}
