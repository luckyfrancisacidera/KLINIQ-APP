using System.Text.Json;

namespace Kliniq.SymptomCatalogBuilder;

internal static class JsonDatasetReader
{
    public static IEnumerable<JsonElement> ReadObjects(string path)
    {
        var text = File.ReadAllText(path);
        var trimmed = text.TrimStart();
        if (trimmed.StartsWith('['))
        {
            using var document = JsonDocument.Parse(text);
            foreach (var item in document.RootElement.EnumerateArray()) yield return item.Clone();
            yield break;
        }

        foreach (var line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            using var document = JsonDocument.Parse(line);
            yield return document.RootElement.Clone();
        }
    }

    public static string GetString(JsonElement item, params string[] names)
    {
        foreach (var property in item.EnumerateObject())
            if (names.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
                return property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() ?? string.Empty : property.Value.ToString();
        return string.Empty;
    }
}
