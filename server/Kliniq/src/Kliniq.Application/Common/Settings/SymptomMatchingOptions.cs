namespace Kliniq.Application.Common.Settings;

public sealed class SymptomMatchingOptions
{
    public const string SectionName = "SymptomMatching";
    public int FuzzyThreshold { get; set; } = 85;
}
