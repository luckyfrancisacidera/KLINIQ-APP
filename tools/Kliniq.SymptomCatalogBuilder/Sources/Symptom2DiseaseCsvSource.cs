// Source: https://www.kaggle.com/datasets/niyarrbarman/symptom2disease
// License shown by Kaggle: CC0 1.0 / Public Domain. Re-check the dataset card before each refresh.
namespace Kliniq.SymptomCatalogBuilder.Sources;

public sealed class Symptom2DiseaseCsvSource(string path) : IDatasetSource
{
    public IEnumerable<(string Disease, IReadOnlyList<string> Symptoms)> Read()
    {
        foreach (var row in CsvDatasetReader.ReadRows(path))
        {
            var disease = CsvDatasetReader.Get(row, "label", "disease", "Disease");
            var text = CsvDatasetReader.Get(row, "text", "symptoms", "Symptoms");
            if (!string.IsNullOrWhiteSpace(disease)) yield return (disease, TextNormalizer.SplitSymptoms(text));
        }
    }
}
