namespace Kliniq.Application.Services;

/// <summary>Deterministic, allocation-conscious similarity helpers used instead of external inference.</summary>
internal static class StringSimilarity
{
    public static int Score(string left, string right)
    {
        if (left.Equals(right, StringComparison.Ordinal)) return 100;
        if (left.Length == 0 || right.Length == 0) return 0;
        if (left[0] != right[0]) return 0;
        if (BigramDice(left, right) < 0.30) return 0;

        var levenshtein = LevenshteinRatio(left, right);
        var jaroWinkler = (int)Math.Round(JaroWinklerSimilarity(left, right) * 100, MidpointRounding.AwayFromZero);
        return Math.Max(levenshtein, jaroWinkler);
    }

    private static int LevenshteinRatio(string left, string right)
    {
        var previous = new int[right.Length + 1];
        var current = new int[right.Length + 1];
        for (var column = 0; column <= right.Length; column++) previous[column] = column;

        for (var row = 1; row <= left.Length; row++)
        {
            current[0] = row;
            for (var column = 1; column <= right.Length; column++)
            {
                var cost = left[row - 1] == right[column - 1] ? 0 : 1;
                current[column] = Math.Min(
                    Math.Min(current[column - 1] + 1, previous[column] + 1),
                    previous[column - 1] + cost);
            }
            (previous, current) = (current, previous);
        }

        var distance = previous[right.Length];
        return (int)Math.Round((1d - distance / (double)Math.Max(left.Length, right.Length)) * 100, MidpointRounding.AwayFromZero);
    }

    private static double JaroWinklerSimilarity(string left, string right)
    {
        var matchDistance = Math.Max(left.Length, right.Length) / 2 - 1;
        matchDistance = Math.Max(0, matchDistance);
        var leftMatches = new bool[left.Length];
        var rightMatches = new bool[right.Length];
        var matches = 0;

        for (var leftIndex = 0; leftIndex < left.Length; leftIndex++)
        {
            var start = Math.Max(0, leftIndex - matchDistance);
            var end = Math.Min(leftIndex + matchDistance + 1, right.Length);
            for (var rightIndex = start; rightIndex < end; rightIndex++)
            {
                if (rightMatches[rightIndex] || left[leftIndex] != right[rightIndex]) continue;
                leftMatches[leftIndex] = true;
                rightMatches[rightIndex] = true;
                matches++;
                break;
            }
        }

        if (matches == 0) return 0;
        var transpositions = 0;
        var cursor = 0;
        for (var leftIndex = 0; leftIndex < left.Length; leftIndex++)
        {
            if (!leftMatches[leftIndex]) continue;
            while (!rightMatches[cursor]) cursor++;
            if (left[leftIndex] != right[cursor]) transpositions++;
            cursor++;
        }

        var jaro = (matches / (double)left.Length + matches / (double)right.Length +
                    (matches - transpositions / 2d) / matches) / 3d;
        var prefix = 0;
        while (prefix < Math.Min(4, Math.Min(left.Length, right.Length)) && left[prefix] == right[prefix]) prefix++;
        return jaro + prefix * 0.1 * (1 - jaro);
    }

    private static double BigramDice(string left, string right)
    {
        if (left.Length < 2 || right.Length < 2) return left == right ? 1 : 0;
        var leftBigrams = Bigrams(left);
        var rightBigrams = Bigrams(right);
        var intersection = leftBigrams.Intersect(rightBigrams, StringComparer.Ordinal).Count();
        return 2d * intersection / (leftBigrams.Count + rightBigrams.Count);
    }

    private static HashSet<string> Bigrams(string value)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < value.Length - 1; index++) result.Add(value.Substring(index, 2));
        return result;
    }
}
