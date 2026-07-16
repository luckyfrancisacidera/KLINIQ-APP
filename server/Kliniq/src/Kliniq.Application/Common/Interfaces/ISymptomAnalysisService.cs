namespace Kliniq.Application.Common.Interfaces
{
    public interface ISymptomAnalysisService
    {
        SymptomAnalysis Analyze(string symptomDescription);
    }

    public sealed record SymptomAnalysis(
        string Urgency,
        string Guidance,
        IReadOnlyList<SymptomSpecialtyMatch> SpecialtyMatches);

    public sealed record SymptomSpecialtyMatch(
        string Specialty,
        int MatchScore,
        IReadOnlyList<string> MatchedSignals);
}
