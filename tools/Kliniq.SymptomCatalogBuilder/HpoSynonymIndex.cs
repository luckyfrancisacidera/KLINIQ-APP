// HPO source: https://github.com/obophenotype/human-phenotype-ontology/releases/latest/download/hp.json
// License/citation instructions: https://hpo.jax.org/app/license
using System.Text.Json;

namespace Kliniq.SymptomCatalogBuilder;

internal sealed class HpoSynonymIndex
{
    private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _synonyms;

    private HpoSynonymIndex(IReadOnlyDictionary<string, IReadOnlyList<string>> synonyms) => _synonyms = synonyms;

    public static HpoSynonymIndex LoadOrEmpty(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"HPO synonym file not found: {path}. Continuing without HPO expansion.");
            return new HpoSynonymIndex(new Dictionary<string, IReadOnlyList<string>>());
        }

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var groups = new List<HashSet<string>>();
        if (document.RootElement.TryGetProperty("graphs", out var graphs))
        {
            foreach (var graph in graphs.EnumerateArray())
            {
                if (!graph.TryGetProperty("nodes", out var nodes)) continue;
                foreach (var node in nodes.EnumerateArray())
                {
                    var terms = new HashSet<string>(StringComparer.Ordinal);
                    if (node.TryGetProperty("lbl", out var label)) Add(terms, label.GetString());
                    if (node.TryGetProperty("meta", out var meta) && meta.TryGetProperty("synonyms", out var synonyms))
                    {
                        foreach (var synonym in synonyms.EnumerateArray())
                            if (synonym.TryGetProperty("val", out var value)) Add(terms, value.GetString());
                    }
                    if (terms.Count > 1) groups.Add(terms);
                }
            }
        }

        var index = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        foreach (var group in groups)
            foreach (var term in group)
                index[term] = group.Where(candidate => candidate != term).OrderBy(candidate => candidate).ToArray();
        return new HpoSynonymIndex(index);
    }

    public IEnumerable<string> Expand(string phrase)
        => _synonyms.TryGetValue(TextNormalizer.Normalize(phrase), out var synonyms) ? synonyms : [];

    private static void Add(HashSet<string> terms, string? value)
    {
        var normalized = TextNormalizer.Normalize(value);
        if (TextNormalizer.IsUsefulPhrase(normalized)) terms.Add(normalized);
    }
}
