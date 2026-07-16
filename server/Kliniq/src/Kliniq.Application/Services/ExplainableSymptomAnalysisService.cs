using System.Text.Json;
using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace Kliniq.Application.Services
{
    /// <summary>
    /// Privacy-first, deterministic symptom-to-specialty matching. It does not diagnose disease.
    /// The reviewed catalog is loaded from an embedded JSON resource; no network or model calls occur.
    /// </summary>
    public sealed class ExplainableSymptomAnalysisService : ISymptomAnalysisService
    {
        private const string CatalogResourceName = "Kliniq.Application.Resources.symptom-catalog.json";
        private const string FuzzyPrefix = "Did you mean";

        private static readonly string[] EmergencySignals =
        [
            "severe chest pain", "chest pain with shortness of breath", "cannot breathe", "cant breathe",
            "difficulty breathing", "face drooping", "one sided weakness", "slurred speech", "unconscious",
            "not responding", "severe bleeding", "vomiting blood", "seizure lasting", "sudden vision loss",
            "suicidal", "want to kill myself", "overdose", "poisoning"
        ];

        private static readonly string[] UrgentSignals =
        [
            "high fever", "persistent vomiting", "dehydrated", "worsening quickly", "severe pain",
            "blood in stool", "blood in urine", "pregnant and bleeding", "new weakness", "confusion"
        ];

        private readonly SpecialtyRule[] _rules;
        private readonly NegationDetector _negationDetector;
        private readonly int _fuzzyThreshold;

        public ExplainableSymptomAnalysisService()
            : this(Options.Create(new SymptomMatchingOptions()), new NegationDetector())
        {
        }

        public ExplainableSymptomAnalysisService(IOptions<SymptomMatchingOptions> options, NegationDetector negationDetector)
        {
            ArgumentNullException.ThrowIfNull(options);
            _negationDetector = negationDetector ?? throw new ArgumentNullException(nameof(negationDetector));
            _fuzzyThreshold = options.Value.FuzzyThreshold;
            if (_fuzzyThreshold is < 70 or > 100)
                throw new InvalidOperationException("SymptomMatching:FuzzyThreshold must be between 70 and 100.");

            using var stream = typeof(ExplainableSymptomAnalysisService).Assembly.GetManifestResourceStream(CatalogResourceName)
                ?? throw new InvalidOperationException($"Embedded symptom catalog '{CatalogResourceName}' is missing.");
            _rules = LoadCatalog(stream)
                .Select(pair => new SpecialtyRule(pair.Key, pair.Value.Select(SymptomText.Tokenize).ToArray()))
                .ToArray();
        }

        internal int CatalogSpecialtyCount => _rules.Length;

        public SymptomAnalysis Analyze(string symptomDescription)
        {
            var input = SymptomText.Tokenize(symptomDescription ?? string.Empty);
            var emergencyMatches = FindAffirmedExactSignals(input, EmergencySignals);
            if (emergencyMatches.Count > 0)
            {
                return new SymptomAnalysis(
                    "Emergency",
                    "Some details may indicate an emergency. Seek immediate in-person emergency care or contact local emergency services now. Do not wait for an online appointment.",
                    [new SymptomSpecialtyMatch("Emergency Medicine", 100, emergencyMatches)]);
            }

            var exactMatches = _rules
                .Select(rule => BuildExactMatch(rule, input))
                .Where(match => match is not null)
                .Cast<SymptomSpecialtyMatch>()
                .ToArray();

            // Fuzzy matching is deliberately global fallback. Once the description contains
            // any affirmed exact signal, fuzzy candidates cannot add unrelated specialties.
            IEnumerable<SymptomSpecialtyMatch> candidateMatches = exactMatches.Length > 0
                ? exactMatches
                : _rules
                    .Select(rule => BuildFuzzyMatch(rule, input))
                    .Where(match => match is not null)
                    .Cast<SymptomSpecialtyMatch>();

            var matches = candidateMatches
                .OrderByDescending(match => match.MatchScore)
                .ThenBy(match => match.Specialty, StringComparer.Ordinal)
                .Take(3)
                .ToList();

            if (matches.Count == 0)
            {
                matches.Add(new SymptomSpecialtyMatch(
                    "General Practice",
                    35,
                    ["The description is broad, so a primary-care assessment is the safest starting point."]));
            }

            var urgentMatches = FindAffirmedExactSignals(input, UrgentSignals);
            var urgency = urgentMatches.Count > 0 ? "Urgent" : "Routine";
            var guidance = urgentMatches.Count > 0
                ? "Arrange an in-person medical assessment as soon as possible. Seek emergency care if symptoms become severe or rapidly worsen."
                : "Use these matches to choose a suitable physician. A licensed clinician must still assess your symptoms and make any diagnosis.";

            return new SymptomAnalysis(urgency, guidance, matches);
        }

        internal static IReadOnlyDictionary<string, string[]> LoadCatalog(Stream stream)
        {
            try
            {
                var catalog = JsonSerializer.Deserialize<Dictionary<string, string[]>>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false
                }) ?? throw new InvalidDataException("The symptom catalog was empty.");

                if (catalog.Count == 0) throw new InvalidDataException("The symptom catalog contains no specialties.");
                var normalized = new Dictionary<string, string[]>(StringComparer.Ordinal);
                foreach (var (specialty, phrases) in catalog)
                {
                    if (string.IsNullOrWhiteSpace(specialty)) throw new InvalidDataException("The symptom catalog contains a blank specialty.");
                    if (phrases is null || phrases.Length == 0) throw new InvalidDataException($"Specialty '{specialty}' contains no phrases.");
                    var cleaned = phrases
                        .Select(SymptomText.Normalize)
                        .Where(value => !string.IsNullOrWhiteSpace(value))
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray();
                    if (cleaned.Length == 0) throw new InvalidDataException($"Specialty '{specialty}' contains no valid phrases.");
                    normalized.Add(specialty.Trim(), cleaned);
                }
                return normalized;
            }
            catch (JsonException exception)
            {
                throw new InvalidDataException("The embedded symptom catalog is malformed JSON.", exception);
            }
        }

        private SymptomSpecialtyMatch? BuildExactMatch(SpecialtyRule rule, TokenizedSymptomText input)
        {
            var matchedSignals = rule.Signals
                .Where(signal => SymptomText.FindExactSpans(input.Tokens, signal.Tokens)
                    .Any(span => !_negationDetector.IsNegated(input.Tokens, span.Start, span.Length)))
                .Select(signal => signal.Normalized)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            return CreateSpecialtyMatch(rule.Specialty, matchedSignals);
        }

        private SymptomSpecialtyMatch? BuildFuzzyMatch(SpecialtyRule rule, TokenizedSymptomText input)
        {
            var matchedSignals = FindFuzzyMatches(rule, input)
                .Select(match => $"{FuzzyPrefix} \"{match.Signal}\"? (matched \"{match.Candidate}\", {match.Score}%)")
                .ToArray();
            return CreateSpecialtyMatch(rule.Specialty, matchedSignals);
        }

        private static SymptomSpecialtyMatch? CreateSpecialtyMatch(string specialty, IReadOnlyList<string> matchedSignals)
        {
            if (matchedSignals.Count == 0) return null;
            var score = Math.Min(95, 45 + matchedSignals.Count * 15);
            return new SymptomSpecialtyMatch(specialty, score, matchedSignals);
        }

        private IReadOnlyList<FuzzySignalMatch> FindFuzzyMatches(SpecialtyRule rule, TokenizedSymptomText input)
        {
            var results = new List<FuzzySignalMatch>();
            foreach (var signal in rule.Signals)
            {
                if (signal.Tokens.Length == 0 || signal.Tokens.Length > input.Tokens.Length) continue;
                FuzzySignalMatch? best = null;
                for (var start = 0; start <= input.Tokens.Length - signal.Tokens.Length; start++)
                {
                    if (_negationDetector.IsNegated(input.Tokens, start, signal.Tokens.Length)) continue;
                    var candidate = string.Join(' ', input.Tokens.Skip(start).Take(signal.Tokens.Length));
                    if (candidate.Equals(signal.Normalized, StringComparison.Ordinal)) continue;
                    var threshold = signal.Normalized.Length <= 4 ? Math.Max(90, _fuzzyThreshold) : _fuzzyThreshold;
                    var score = StringSimilarity.Score(candidate, signal.Normalized);
                    if (score < threshold) continue;
                    if (best is null || score > best.Score)
                        best = new FuzzySignalMatch(signal.Normalized, candidate, score);
                }
                if (best is not null) results.Add(best);
            }

            return results
                .OrderByDescending(match => match.Score)
                .ThenBy(match => match.Signal, StringComparer.Ordinal)
                .GroupBy(match => match.Signal, StringComparer.Ordinal)
                .Select(group => group.First())
                .Take(3)
                .ToArray();
        }

        private IReadOnlyList<string> FindAffirmedExactSignals(TokenizedSymptomText input, IEnumerable<string> signals)
        {
            var matches = new List<string>();
            foreach (var signal in signals)
            {
                var normalized = SymptomText.Tokenize(signal);
                var spans = SymptomText.FindExactSpans(input.Tokens, normalized.Tokens);
                if (spans.Any(span => !_negationDetector.IsNegated(input.Tokens, span.Start, span.Length)))
                    matches.Add(normalized.Normalized);
            }
            return matches.Distinct(StringComparer.Ordinal).ToArray();
        }

        private sealed record SpecialtyRule(string Specialty, TokenizedSymptomText[] Signals);
        private sealed record FuzzySignalMatch(string Signal, string Candidate, int Score);
    }
}
