// Source: https://www.kaggle.com/datasets/dhivyeshrk/diseases-and-symptoms-dataset
// License shown by Kaggle: World Bank Dataset Terms of Use. Verify downstream/redistribution rights before committing derived data.
namespace Kliniq.SymptomCatalogBuilder.Sources;

public sealed class DiseasesAndSymptomsCsvSource(string path) : IDatasetSource
{
    private static readonly string[] DiseaseColumns = ["diseases", "disease", "Disease", "prognosis", "label"];

    public IEnumerable<(string Disease, IReadOnlyList<string> Symptoms)> Read()
    {
        foreach (var row in CsvDatasetReader.ReadRows(path))
        {
            var disease = CsvDatasetReader.Get(row, DiseaseColumns);
            var symptoms = row
                .Where(pair => !DiseaseColumns.Contains(pair.Key, StringComparer.OrdinalIgnoreCase) && IsPresent(pair.Value))
                .Select(pair => TextNormalizer.Normalize(pair.Key.Replace('_', ' ')))
                .Where(TextNormalizer.IsUsefulPhrase)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (!string.IsNullOrWhiteSpace(disease)) yield return (disease, symptoms);
        }
    }

    private static bool IsPresent(string value)
        => value.Trim() is "1" or "1.0" || bool.TryParse(value, out var result) && result;
}
