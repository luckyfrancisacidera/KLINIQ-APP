using System.Text.Json;
using Kliniq.SymptomCatalogBuilder;
using Kliniq.SymptomCatalogBuilder.Sources;

var repositoryRoot = FindRepositoryRoot(AppContext.BaseDirectory);
var toolRoot = Path.Combine(repositoryRoot, "tools", "Kliniq.SymptomCatalogBuilder");
var dataRoot = Path.Combine(toolRoot, "data");
var outputPath = Path.Combine(repositoryRoot, "server", "Kliniq", "src", "Kliniq.Application", "Resources", "symptom-catalog.json");
var baselinePath = Path.Combine(dataRoot, "baseline-hardcoded-catalog.json");
var diffPath = Path.Combine(toolRoot, "catalog-diff.md");
var unmappedPath = Path.Combine(dataRoot, "unmapped-diseases.txt");

var baseline = ReadCatalog(baselinePath);
var catalog = baseline.ToDictionary(pair => pair.Key, pair => new HashSet<string>(pair.Value, StringComparer.Ordinal), StringComparer.Ordinal);
var map = new SpecialtyMap(Path.Combine(dataRoot, "icd10-specialty-map.json"));
var hpo = HpoSynonymIndex.LoadOrEmpty(Path.Combine(dataRoot, "hp.json"));
var stoplist = new HashSet<string>(JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Combine(dataRoot, "generic-symptom-stoplist.json"))) ?? [], StringComparer.Ordinal);
var sources = DiscoverSources(dataRoot).ToArray();
var unmapped = new HashSet<string>(StringComparer.Ordinal);
var mappedRows = 0;

if (sources.Length == 0)
{
    throw new InvalidOperationException(
        "No local dataset exports were found. Drop at least one reviewed CSV/JSON export into the data folder before running the builder. " +
        "No production catalog was changed.");
}

foreach (var source in sources)
{
    foreach (var (disease, symptoms) in source.Read())
    {
        if (!map.TryMap(disease, out var specialty))
        {
            unmapped.Add(TextNormalizer.Normalize(disease));
            continue;
        }

        mappedRows++;
        if (!catalog.TryGetValue(specialty, out var phrases)) catalog[specialty] = phrases = new HashSet<string>(StringComparer.Ordinal);
        foreach (var symptom in symptoms.Select(TextNormalizer.Normalize).Where(TextNormalizer.IsUsefulPhrase))
        {
            if (stoplist.Contains(symptom)) continue;
            phrases.Add(symptom);
            foreach (var synonym in hpo.Expand(symptom))
                if (!stoplist.Contains(synonym)) phrases.Add(synonym);
        }
    }
}

var result = catalog.OrderBy(pair => pair.Key).ToDictionary(
    pair => pair.Key,
    pair => pair.Value.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
    StringComparer.Ordinal);
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);
File.WriteAllLines(unmappedPath, new[] { "# Diseases without a reviewed specialty mapping. Do not guess." }.Concat(unmapped.OrderBy(value => value)));
var diff = BuildDiff(baseline, result, mappedRows, unmapped.Count, sources.Length);
File.WriteAllText(diffPath, diff);
Console.WriteLine(diff);
Console.WriteLine($"Generated catalog: {outputPath}");
Console.WriteLine("Review symptom-catalog.json, catalog-diff.md, and unmapped-diseases.txt before committing.");

static IEnumerable<IDatasetSource> DiscoverSources(string dataRoot)
{
    foreach (var path in Existing(dataRoot, "symptom2disease.csv")) yield return new Symptom2DiseaseCsvSource(path);
    foreach (var path in Existing(dataRoot, "gretel-symptom-to-diagnosis.jsonl", "gretel-symptom-to-diagnosis.json", "gretel-symptom-to-diagnosis.csv")) yield return new GretelAiSymptomToDiagnosisSource(path);
    foreach (var path in Existing(dataRoot, "disease-symptom-description.csv")) yield return new DiseaseSymptomDescriptionCsvSource(path);
    foreach (var path in Existing(dataRoot, "diseases-and-symptoms.csv")) yield return new DiseasesAndSymptomsCsvSource(path);
    foreach (var path in Existing(dataRoot, "quyenanh-diseases-symptoms.json", "quyenanh-diseases-symptoms.csv")) yield return new QuyenAnhDiseasesSymptomsSource(path);
}

static IEnumerable<string> Existing(string root, params string[] names)
    => names.Select(name => Path.Combine(root, name)).Where(File.Exists);

static Dictionary<string, string[]> ReadCatalog(string path)
    => JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText(path))
       ?? throw new InvalidDataException($"Catalog is malformed: {path}");

static string BuildDiff(IReadOnlyDictionary<string, string[]> baseline, IReadOnlyDictionary<string, string[]> result, int mappedRows, int unmappedCount, int sourceCount)
{
    var writer = new System.Text.StringBuilder();
    writer.AppendLine("# Symptom catalog review diff").AppendLine();
    writer.AppendLine($"- Local sources read: {sourceCount}");
    writer.AppendLine($"- Mapped dataset rows: {mappedRows}");
    writer.AppendLine($"- Unmapped diseases: {unmappedCount}").AppendLine();
    writer.AppendLine("| Specialty | Baseline | Generated | Added |");
    writer.AppendLine("|---|---:|---:|---:|");
    foreach (var specialty in result.Keys.OrderBy(value => value))
    {
        var before = baseline.GetValueOrDefault(specialty) ?? [];
        var added = result[specialty].Except(before, StringComparer.Ordinal).ToArray();
        writer.AppendLine($"| {specialty} | {before.Length} | {result[specialty].Length} | {added.Length} |");
    }
    writer.AppendLine().AppendLine("## Added phrases").AppendLine();
    foreach (var specialty in result.Keys.OrderBy(value => value))
    {
        var before = baseline.GetValueOrDefault(specialty) ?? [];
        var added = result[specialty].Except(before, StringComparer.Ordinal).OrderBy(value => value).ToArray();
        if (added.Length == 0) continue;
        writer.AppendLine($"### {specialty}").AppendLine();
        writer.AppendLine(string.Join(", ", added.Select(value => $"`{value}`"))).AppendLine();
    }
    return writer.ToString();
}

static string FindRepositoryRoot(string start)
{
    var current = new DirectoryInfo(start);
    while (current is not null)
    {
        if (Directory.Exists(Path.Combine(current.FullName, "server", "Kliniq")) && Directory.Exists(Path.Combine(current.FullName, "tools")))
            return current.FullName;
        current = current.Parent;
    }
    throw new DirectoryNotFoundException("Could not locate the KLINIQ repository root.");
}
