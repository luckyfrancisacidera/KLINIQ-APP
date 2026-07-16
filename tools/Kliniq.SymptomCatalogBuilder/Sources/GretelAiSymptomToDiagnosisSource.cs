// Source: https://huggingface.co/datasets/gretelai/symptom_to_diagnosis
// License: Apache-2.0, as declared in the Hugging Face dataset card.
using System.Text.Json;

namespace Kliniq.SymptomCatalogBuilder.Sources;

public sealed class GretelAiSymptomToDiagnosisSource(string path) : IDatasetSource
{
    public IEnumerable<(string Disease, IReadOnlyList<string> Symptoms)> Read()
    {
        if (Path.GetExtension(path).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var row in CsvDatasetReader.ReadRows(path))
            {
                var disease = CsvDatasetReader.Get(row, "output_text", "diagnosis", "disease");
                var text = CsvDatasetReader.Get(row, "input_text", "symptoms", "text");
                if (!string.IsNullOrWhiteSpace(disease)) yield return (disease, TextNormalizer.SplitSymptoms(text));
            }
            yield break;
        }

        foreach (var item in JsonDatasetReader.ReadObjects(path))
        {
            var disease = JsonDatasetReader.GetString(item, "output_text", "diagnosis", "disease");
            var text = JsonDatasetReader.GetString(item, "input_text", "symptoms", "text");
            if (!string.IsNullOrWhiteSpace(disease)) yield return (disease, TextNormalizer.SplitSymptoms(text));
        }
    }
}
