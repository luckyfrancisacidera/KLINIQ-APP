using Kliniq.Application.Services;

namespace Kliniq.Application.Tests;

public sealed class NegationDetectorTests
{
    private readonly NegationDetector _detector = new();

    [Fact]
    public void IsNegated_PreTriggerWithinWindow_ReturnsTrue()
    {
        var text = SymptomText.Tokenize("The patient denies any chest pain today.");
        var span = Assert.Single(SymptomText.FindExactSpans(text.Tokens, SymptomText.Tokenize("chest pain").Tokens));

        Assert.True(_detector.IsNegated(text.Tokens, span.Start, span.Length));
    }

    [Fact]
    public void IsNegated_ClauseBoundaryPreventsNegationLeak_ReturnsFalse()
    {
        var text = SymptomText.Tokenize("No chest pain, but I have a rash.");
        var span = Assert.Single(SymptomText.FindExactSpans(text.Tokens, SymptomText.Tokenize("rash").Tokens));

        Assert.False(_detector.IsNegated(text.Tokens, span.Start, span.Length));
    }

    [Fact]
    public void IsNegated_PostTrigger_ReturnsTrue()
    {
        var text = SymptomText.Tokenize("Chest pain was ruled out.");
        var span = Assert.Single(SymptomText.FindExactSpans(text.Tokens, SymptomText.Tokenize("chest pain").Tokens));

        Assert.True(_detector.IsNegated(text.Tokens, span.Start, span.Length));
    }

    [Fact]
    public void IsNegated_NotSureIf_IsUncertaintyNotNegation()
    {
        var text = SymptomText.Tokenize("I am not sure if I have chest pain.");
        var span = Assert.Single(SymptomText.FindExactSpans(text.Tokens, SymptomText.Tokenize("chest pain").Tokens));

        Assert.False(_detector.IsNegated(text.Tokens, span.Start, span.Length));
    }

    [Fact]
    public void IsNegated_NotWithout_IsDoubleNegationAndKept()
    {
        var text = SymptomText.Tokenize("I am not without chest pain.");
        var span = Assert.Single(SymptomText.FindExactSpans(text.Tokens, SymptomText.Tokenize("chest pain").Tokens));

        Assert.False(_detector.IsNegated(text.Tokens, span.Start, span.Length));
    }
}
