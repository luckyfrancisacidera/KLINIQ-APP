using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kliniq.SymptomCatalogBuilder;

internal static partial class TextNormalizer
{
    private static readonly string[] NarrativePrefixes =
    [
        "i have ", "i ve had ", "ive had ", "i feel ", "i am ", "i m ", "im ",
        "experiencing ", "suffering from ", "there is ", "there are ", "my "
    ];

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var decomposed = value.Normalize(NormalizationForm.FormD);
        var withoutMarks = new string(decomposed
            .Where(character => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        var normalized = PunctuationRegex().Replace(withoutMarks.ToLowerInvariant(), " ");
        return WhitespaceRegex().Replace(normalized, " ").Trim();
    }

    public static IReadOnlyList<string> SplitSymptoms(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return [];

        return ClauseSeparatorRegex().Split(value)
            .Select(Normalize)
            .Select(RemoveNarrativePrefix)
            .Where(IsUsefulPhrase)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static bool IsUsefulPhrase(string phrase)
    {
        if (phrase.Length < 3 || phrase.Length > 90) return false;
        var tokenCount = phrase.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return tokenCount is >= 1 and <= 10;
    }

    private static string RemoveNarrativePrefix(string phrase)
    {
        foreach (var prefix in NarrativePrefixes)
        {
            if (phrase.StartsWith(prefix, StringComparison.Ordinal))
                return phrase[prefix.Length..].Trim();
        }
        return phrase;
    }

    [GeneratedRegex(@"[^a-z0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex PunctuationRegex();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"(?:\r?\n|[;,|•]+|\.(?:\s+|$)|\s+(?:and also|as well as)\s+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ClauseSeparatorRegex();
}
