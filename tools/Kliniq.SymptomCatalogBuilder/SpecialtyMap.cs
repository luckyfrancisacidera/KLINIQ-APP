using System.Text.Json;

namespace Kliniq.SymptomCatalogBuilder;

internal sealed class SpecialtyMap
{
    private readonly IReadOnlyDictionary<string, string> _chapterSpecialties;
    private readonly IReadOnlyDictionary<string, DiseaseMapEntry> _diseases;

    public SpecialtyMap(string path)
    {
        var model = JsonSerializer.Deserialize<MapFile>(File.ReadAllText(path), JsonOptions())
            ?? throw new InvalidDataException($"Unable to parse specialty map: {path}");

        if (model.ChapterSpecialties.Count == 0 || model.Diseases.Count == 0)
            throw new InvalidDataException($"Specialty map contains no chapter or disease mappings: {path}");

        _chapterSpecialties = new Dictionary<string, string>(model.ChapterSpecialties, StringComparer.OrdinalIgnoreCase);
        _diseases = model.Diseases.ToDictionary(
            pair => TextNormalizer.Normalize(pair.Key),
            pair => pair.Value,
            StringComparer.Ordinal);
    }

    public bool TryMap(string disease, out string specialty)
    {
        var key = TextNormalizer.Normalize(StripDatasetSuffix(disease));
        if (!_diseases.TryGetValue(key, out var entry))
        {
            specialty = string.Empty;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(entry.Specialty))
        {
            specialty = entry.Specialty.Trim();
            return true;
        }

        if (!string.IsNullOrWhiteSpace(entry.Chapter) &&
            _chapterSpecialties.TryGetValue(entry.Chapter.Trim(), out var chapterSpecialty) &&
            !string.IsNullOrWhiteSpace(chapterSpecialty))
        {
            specialty = chapterSpecialty.Trim();
            return true;
        }

        specialty = string.Empty;
        return false;
    }

    private static string StripDatasetSuffix(string disease)
        => System.Text.RegularExpressions.Regex.Replace(disease, @"-\d+$", string.Empty);

    private static JsonSerializerOptions JsonOptions() => new() { PropertyNameCaseInsensitive = true };

    private sealed class MapFile
    {
        public Dictionary<string, string> ChapterSpecialties { get; init; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, DiseaseMapEntry> Diseases { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed class DiseaseMapEntry
    {
        public string Chapter { get; init; } = string.Empty;
        public string Specialty { get; init; } = string.Empty;
    }
}
