// Source: https://www.kaggle.com/datasets/itachi9604/disease-symptom-description-dataset
// License shown by Kaggle: CC BY-SA 4.0. Preserve attribution/share-alike obligations for derived data.
namespace Kliniq.SymptomCatalogBuilder.Sources;

public sealed class DiseaseSymptomDescriptionCsvSource(string path) : IDatasetSource
{
    public IEnumerable<(string Disease, IReadOnlyList<string> Symptoms)> Read()
    {
        foreach (var row in CsvDatasetReader.ReadRows(path))
        {
            var disease = CsvDatasetReader.Get(row, "Disease", "disease", "prognosis");
            var symptoms = row
                .Where(pair => pair.Key.StartsWith("Symptom", StringComparison.OrdinalIgnoreCase))
                .SelectMany(pair => TextNormalizer.SplitSymptoms(pair.Value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (!string.IsNullOrWhiteSpace(disease)) yield return (disease, symptoms);
        }
    }
}
