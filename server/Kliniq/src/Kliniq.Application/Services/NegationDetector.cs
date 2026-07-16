namespace Kliniq.Application.Services;

/// <summary>
/// Small NegEx-style detector. A symptom is suppressed when an odd number of explicit
/// negation triggers appears in the same clause within the configured token window.
/// Uncertainty such as "not sure if" and double negation are intentionally treated as
/// non-negated so KLINIQ keeps the possible symptom in routing results.
/// </summary>
public sealed class NegationDetector
{
    private const int BeforeWindow = 7;
    private const int AfterWindow = 4;

    private static readonly string[][] PreTriggers =
    [
        ["no"], ["denies"], ["denied"], ["without"], ["never"], ["absence", "of"],
        ["not", "experiencing"], ["not", "having"], ["do", "not", "have"],
        ["does", "not", "have"], ["did", "not", "have"], ["dont", "have"],
        ["doesnt", "have"], ["didnt", "have"], ["has", "no"], ["have", "no"],
        ["negative", "for"], ["free", "of"], ["ruled", "out"], ["rule", "out"]
    ];

    private static readonly string[][] PostTriggers =
    [
        ["absent"], ["not", "present"], ["ruled", "out"], ["was", "ruled", "out"],
        ["is", "ruled", "out"], ["not", "seen"]
    ];

    private static readonly string[][] UncertaintyTriggers =
    [
        ["not", "sure", "if"], ["not", "certain", "if"], ["unsure", "if"],
        ["unsure", "whether"], ["cannot", "tell", "if"], ["cant", "tell", "if"],
        ["cannot", "rule", "out"], ["cant", "rule", "out"]
    ];

    private static readonly string[][] DoubleNegationTriggers =
    [
        ["not", "without"], ["dont", "not", "have"], ["doesnt", "not", "have"],
        ["do", "not", "not", "have"], ["does", "not", "not", "have"]
    ];

    private static readonly HashSet<string> ClauseBoundaries = new(StringComparer.Ordinal)
    {
        "but", "however", "although", "though", "except", "yet", "nevertheless"
    };

    public bool IsNegated(IReadOnlyList<string> tokens, int symptomStart, int symptomLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(symptomStart);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(symptomLength);
        if (symptomStart + symptomLength > tokens.Count) throw new ArgumentOutOfRangeException(nameof(symptomLength));

        var beforeStart = Math.Max(0, symptomStart - BeforeWindow);
        for (var index = symptomStart - 1; index >= beforeStart; index--)
        {
            if (!ClauseBoundaries.Contains(tokens[index])) continue;
            beforeStart = index + 1;
            break;
        }

        var afterEnd = Math.Min(tokens.Count, symptomStart + symptomLength + AfterWindow);
        for (var index = symptomStart + symptomLength; index < afterEnd; index++)
        {
            if (!ClauseBoundaries.Contains(tokens[index])) continue;
            afterEnd = index;
            break;
        }

        var before = Slice(tokens, beforeStart, symptomStart - beforeStart);
        var after = Slice(tokens, symptomStart + symptomLength, afterEnd - symptomStart - symptomLength);

        if (ContainsAny(before, UncertaintyTriggers) || ContainsAny(before, DoubleNegationTriggers)) return false;

        var triggerCount = CountTriggers(before, PreTriggers) + CountTriggers(after, PostTriggers);
        return triggerCount % 2 == 1;
    }

    private static string[] Slice(IReadOnlyList<string> tokens, int start, int length)
    {
        var result = new string[length];
        for (var index = 0; index < length; index++) result[index] = tokens[start + index];
        return result;
    }

    private static bool ContainsAny(IReadOnlyList<string> tokens, IEnumerable<string[]> triggers)
        => triggers.Any(trigger => Contains(tokens, trigger));

    private static int CountTriggers(IReadOnlyList<string> tokens, IEnumerable<string[]> triggers)
    {
        // Prefer the longest trigger at each token so overlapping forms such as
        // "ruled out" and "was ruled out" count as one semantic negation.
        var ordered = triggers.OrderByDescending(trigger => trigger.Length).ToArray();
        var count = 0;
        var index = 0;
        while (index < tokens.Count)
        {
            var matchedLength = 0;
            foreach (var trigger in ordered)
            {
                if (trigger.Length <= matchedLength || index + trigger.Length > tokens.Count) continue;
                var matches = true;
                for (var offset = 0; offset < trigger.Length; offset++)
                {
                    if (tokens[index + offset].Equals(trigger[offset], StringComparison.Ordinal)) continue;
                    matches = false;
                    break;
                }
                if (matches) matchedLength = trigger.Length;
            }

            if (matchedLength > 0)
            {
                count++;
                index += matchedLength;
            }
            else
            {
                index++;
            }
        }
        return count;
    }

    private static bool Contains(IReadOnlyList<string> tokens, IReadOnlyList<string> trigger)
        => Count(tokens, trigger) > 0;

    private static int Count(IReadOnlyList<string> tokens, IReadOnlyList<string> trigger)
    {
        if (trigger.Count == 0 || trigger.Count > tokens.Count) return 0;
        var count = 0;
        for (var start = 0; start <= tokens.Count - trigger.Count; start++)
        {
            var matches = true;
            for (var offset = 0; offset < trigger.Count; offset++)
            {
                if (tokens[start + offset].Equals(trigger[offset], StringComparison.Ordinal)) continue;
                matches = false;
                break;
            }
            if (matches) count++;
        }
        return count;
    }
}
