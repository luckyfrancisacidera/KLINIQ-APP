namespace Kliniq.SymptomCatalogBuilder;

public interface IDatasetSource
{
    IEnumerable<(string Disease, IReadOnlyList<string> Symptoms)> Read();
}
