// Source: https://huggingface.co/datasets/QuyenAnhDE/Diseases_Symptoms
// License: no license was declared on the dataset card when this parser was authored (2026-07-11).
// Do not redistribute the raw export or derived phrases until the dataset owner clarifies usage rights.
namespace Kliniq.SymptomCatalogBuilder.Sources;

public sealed class QuyenAnhDiseasesSymptomsSource(string path) : IDatasetSource
{
    public IEnumerable<(string Disease, IReadOnlyList<string> Symptoms)> Read()
    {
        if (Path.GetExtension(path).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var row in CsvDatasetReader.ReadRows(path))
            {
                var disease = CsvDatasetReader.Get(row, "Name", "name", "Disease", "disease");
                var symptoms = CsvDatasetReader.Get(row, "Symptoms", "symptoms");
                if (!string.IsNullOrWhiteSpace(disease)) yield return (disease, TextNormalizer.SplitSymptoms(symptoms));
            }
            yield break;
        }

        foreach (var item in JsonDatasetReader.ReadObjects(path))
        {
            var disease = JsonDatasetReader.GetString(item, "Name", "name", "Disease", "disease");
            var symptoms = JsonDatasetReader.GetString(item, "Symptoms", "symptoms");
            if (!string.IsNullOrWhiteSpace(disease)) yield return (disease, TextNormalizer.SplitSymptoms(symptoms));
        }
    }
}
