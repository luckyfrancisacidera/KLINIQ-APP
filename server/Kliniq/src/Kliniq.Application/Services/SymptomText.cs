using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kliniq.Application.Services;

internal static partial class SymptomText
{
    public static TokenizedSymptomText Tokenize(string value)
    {
        var normalized = Normalize(value);
        return new TokenizedSymptomText(normalized, normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var withoutMarks = new string(decomposed
            .Where(character => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        var punctuationRemoved = NonTokenRegex().Replace(withoutMarks.ToLowerInvariant(), " ");
        return WhitespaceRegex().Replace(punctuationRemoved, " ").Trim();
    }

    public static IReadOnlyList<TokenSpan> FindExactSpans(IReadOnlyList<string> tokens, IReadOnlyList<string> phraseTokens)
    {
        if (phraseTokens.Count == 0 || phraseTokens.Count > tokens.Count) return [];
        var spans = new List<TokenSpan>();
        for (var start = 0; start <= tokens.Count - phraseTokens.Count; start++)
        {
            var matches = true;
            for (var offset = 0; offset < phraseTokens.Count; offset++)
            {
                if (!tokens[start + offset].Equals(phraseTokens[offset], StringComparison.Ordinal))
                {
                    matches = false;
                    break;
                }
            }
            if (matches) spans.Add(new TokenSpan(start, phraseTokens.Count));
        }
        return spans;
    }

    [GeneratedRegex(@"[^a-z0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex NonTokenRegex();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespaceRegex();
}

internal sealed record TokenizedSymptomText(string Normalized, string[] Tokens);
internal readonly record struct TokenSpan(int Start, int Length)
{
    public int EndExclusive => Start + Length;
}
