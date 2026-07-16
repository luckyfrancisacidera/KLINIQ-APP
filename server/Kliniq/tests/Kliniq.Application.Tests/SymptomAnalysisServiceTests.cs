using System.Text;
using Kliniq.Application.Common.Settings;
using Kliniq.Application.Services;
using Microsoft.Extensions.Options;

namespace Kliniq.Application.Tests;

public sealed class SymptomAnalysisServiceTests
{
    private readonly ExplainableSymptomAnalysisService _service = new();

    [Fact]
    public void Constructor_LoadsReviewedCatalogFromEmbeddedResource()
    {
        Assert.Equal(14, _service.CatalogSpecialtyCount);

        var result = _service.Analyze("I have double vision and floaters.");

        Assert.Equal("Ophthalmology", result.SpecialtyMatches[0].Specialty);
        Assert.Contains("double vision", result.SpecialtyMatches[0].MatchedSignals);
    }

    [Fact]
    public void LoadCatalog_MalformedJson_FailsLoudly()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{ not-json"));

        Assert.Throws<InvalidDataException>(() => ExplainableSymptomAnalysisService.LoadCatalog(stream));
    }

    [Fact]
    public void Analyze_RashDescription_SuggestsDermatology()
    {
        var result = _service.Analyze("I have an itchy red rash on both arms for three days.");

        Assert.Equal("Routine", result.Urgency);
        Assert.Equal("Dermatology", result.SpecialtyMatches[0].Specialty);
        Assert.Contains("rash", result.SpecialtyMatches[0].MatchedSignals);
    }

    [Fact]
    public void Analyze_NegatedBreathingDifficulty_DoesNotMatchPulmonologyOrEmergency()
    {
        var result = _service.Analyze("I do not have difficulty breathing.");

        Assert.Equal("Routine", result.Urgency);
        Assert.DoesNotContain(result.SpecialtyMatches, match => match.Specialty == "Pulmonology");
    }

    [Fact]
    public void Analyze_MixedNegatedAndAffirmedSymptoms_OnlyScoresAffirmedPhrase()
    {
        var result = _service.Analyze("No chest pain, but I do have a rash.");

        Assert.Equal("Dermatology", result.SpecialtyMatches[0].Specialty);
        Assert.DoesNotContain(result.SpecialtyMatches, match => match.Specialty == "Cardiology");
    }

    [Fact]
    public void Analyze_UncertaintyIsNotTreatedAsNegation()
    {
        // Expected behavior: uncertainty is retained for safety; it is not a denial of the symptom.
        var result = _service.Analyze("I am not sure if I have chest pain.");

        Assert.Contains(result.SpecialtyMatches, match => match.Specialty == "Cardiology");
    }

    [Fact]
    public void Analyze_DoubleNegationIsNotTreatedAsSymptomAbsence()
    {
        // Expected behavior: "not without" is ambiguous/presence-leaning, so routing keeps the symptom.
        var result = _service.Analyze("I am not without chest pain today.");

        Assert.Contains(result.SpecialtyMatches, match => match.Specialty == "Cardiology");
    }

    [Fact]
    public void Analyze_AffirmedEmergencyWarning_ReturnsEmergencyGuidanceBeforeNormalMatching()
    {
        var result = _service.Analyze("I have severe chest pain and cannot breathe.");

        Assert.Equal("Emergency", result.Urgency);
        Assert.Single(result.SpecialtyMatches);
        Assert.Equal("Emergency Medicine", result.SpecialtyMatches[0].Specialty);
        Assert.Contains("immediate", result.Guidance, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Analyze_AffirmedUrgentWarning_ReturnsUrgentGuidance()
    {
        var result = _service.Analyze("I have high fever and persistent vomiting.");

        Assert.Equal("Urgent", result.Urgency);
        Assert.Contains("as soon as possible", result.Guidance, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Analyze_NegatedEmergencyWarning_DoesNotShortCircuitNormalMatching()
    {
        var result = _service.Analyze("No severe chest pain, but I do have a rash.");

        Assert.Equal("Routine", result.Urgency);
        Assert.Equal("Dermatology", result.SpecialtyMatches[0].Specialty);
        Assert.DoesNotContain(result.SpecialtyMatches, match => match.Specialty == "Emergency Medicine");
    }

    [Fact]
    public void Analyze_NegatedUrgentWarning_DoesNotSetUrgent()
    {
        var result = _service.Analyze("I do not have high fever, but I do have a rash.");

        Assert.Equal("Routine", result.Urgency);
        Assert.Equal("Dermatology", result.SpecialtyMatches[0].Specialty);
    }

    [Fact]
    public void Analyze_MisspelledEmergencyPhrase_DoesNotFuzzyTriggerEmergency()
    {
        // Emergency and urgent detection intentionally remains exact-only.
        var result = _service.Analyze("I have sevre chest pain.");

        Assert.NotEqual("Emergency", result.Urgency);
        Assert.DoesNotContain(result.SpecialtyMatches, match => match.Specialty == "Emergency Medicine");
    }

    [Fact]
    public void Analyze_ManySignals_CapsScoreAtNinetyFive()
    {
        var result = _service.Analyze("I have a rash, itchy skin, hives, eczema, acne, blisters, and flaky skin.");

        Assert.Equal("Dermatology", result.SpecialtyMatches[0].Specialty);
        Assert.Equal(95, result.SpecialtyMatches[0].MatchScore);
    }

    [Fact]
    public void Analyze_MoreThanThreeSpecialties_ReturnsOnlyTopThreeWithDeterministicOrdering()
    {
        var result = _service.Analyze("I have rash, hives, itchy skin, headache, migraine, cough, wheezing, and heartburn.");

        Assert.Equal(3, result.SpecialtyMatches.Count);
        Assert.Equal(["Dermatology", "Neurology", "Pulmonology"], result.SpecialtyMatches.Select(match => match.Specialty));
    }

    [Fact]
    public void Analyze_UnknownSymptoms_FallsBackToGeneralPractice()
    {
        var result = _service.Analyze("I feel unusual and I am not sure which doctor I need.");

        Assert.Equal("General Practice", result.SpecialtyMatches[0].Specialty);
    }

    [Fact]
    public void Constructor_InvalidFuzzyThreshold_FailsLoudly()
    {
        Assert.Throws<InvalidOperationException>(() => new ExplainableSymptomAnalysisService(
            Options.Create(new SymptomMatchingOptions { FuzzyThreshold = 69 }),
            new NegationDetector()));
    }

    [Fact]
    public void Analyze_ExactSignalPresent_DoesNotAddUnrelatedFuzzySpecialties()
    {
        var result = _service.Analyze("I have a rash on both arms.");

        Assert.Equal("Dermatology", result.SpecialtyMatches[0].Specialty);
        Assert.DoesNotContain(result.SpecialtyMatches.SelectMany(match => match.MatchedSignals),
            signal => signal.StartsWith("Did you mean", StringComparison.Ordinal));
    }

    [Fact]
    public void Analyze_StrictFuzzyThreshold_CanDisableNearMatch()
    {
        var service = new ExplainableSymptomAnalysisService(
            Options.Create(new SymptomMatchingOptions { FuzzyThreshold = 100 }),
            new NegationDetector());

        var result = service.Analyze("I have migrane.");

        Assert.Equal("General Practice", result.SpecialtyMatches[0].Specialty);
        Assert.DoesNotContain(result.SpecialtyMatches, match => match.Specialty == "Neurology");
    }

    [Theory]
    [MemberData(nameof(FuzzyMisspellings))]
    public void Analyze_RealisticMisspelling_UsesFlaggedFuzzyMatch(string specialty, string misspelling, string expectedPhrase)
    {
        var service = new ExplainableSymptomAnalysisService(
            Options.Create(new SymptomMatchingOptions { FuzzyThreshold = 85 }),
            new NegationDetector());

        var result = service.Analyze($"I have {misspelling}.");
        var match = Assert.Single(result.SpecialtyMatches,item => item.Specialty == specialty);

        Assert.Contains(match.MatchedSignals, signal =>
            signal.StartsWith("Did you mean", StringComparison.Ordinal) &&
            signal.Contains($"\"{expectedPhrase}\"", StringComparison.Ordinal));
    }

    public static TheoryData<string, string, string> FuzzyMisspellings => new()
    {
        { "Cardiology", "palpatations", "palpitations" },
        { "Cardiology", "irreglar heartbeat", "irregular heartbeat" },
        { "Cardiology", "chest presure", "chest pressure" },
        { "Cardiology", "racing hert", "racing heart" },
        { "Cardiology", "ankle sweling", "ankle swelling" },

        { "Pulmonology", "persistant coff", "persistent cough" },
        { "Pulmonology", "weezing", "wheezing" },
        { "Pulmonology", "shortnes of breath", "shortness of breath" },
        { "Pulmonology", "chest tightnes", "chest tightness" },
        { "Pulmonology", "breathlesness", "breathlessness" },

        { "Dermatology", "rahs", "rash" },
        { "Dermatology", "itchy skn", "itchy skin" },
        { "Dermatology", "ecsema", "eczema" },
        { "Dermatology", "hivs", "hives" },
        { "Dermatology", "blistrs", "blisters" },

        { "Neurology", "migrane", "migraine" },
        { "Neurology", "hedache", "headache" },
        { "Neurology", "numbnes", "numbness" },
        { "Neurology", "dizzines", "dizziness" },
        { "Neurology", "seizur", "seizure" },

        { "Gastroenterology", "abdomnal pain", "abdominal pain" },
        { "Gastroenterology", "diarhea", "diarrhea" },
        { "Gastroenterology", "constiption", "constipation" },
        { "Gastroenterology", "hartburn", "heartburn" },
        { "Gastroenterology", "bloatng", "bloating" },

        { "Pediatrics", "infnt", "infant" },
        { "Pediatrics", "todler", "toddler" },
        { "Pediatrics", "newbron", "newborn" },
        { "Pediatrics", "pediatrik", "pediatric" },
        { "Pediatrics", "babby", "baby" },

        { "Obstetrics and Gynecology", "pregnacy", "pregnancy" },
        { "Obstetrics and Gynecology", "menstral", "menstrual" },
        { "Obstetrics and Gynecology", "pelvc pain", "pelvic pain" },
        { "Obstetrics and Gynecology", "vagnal bleading", "vaginal bleeding" },
        { "Obstetrics and Gynecology", "prenatl", "prenatal" },

        { "Orthopedics", "joint pan", "joint pain" },
        { "Orthopedics", "sholder pain", "shoulder pain" },
        { "Orthopedics", "fractur", "fracture" },
        { "Orthopedics", "sports injry", "sports injury" },
        { "Orthopedics", "joint stifness", "joint stiffness" },

        { "Otolaryngology (ENT)", "ear pan", "ear pain" },
        { "Otolaryngology (ENT)", "hearing los", "hearing loss" },
        { "Otolaryngology (ENT)", "sore throt", "sore throat" },
        { "Otolaryngology (ENT)", "nasal congstion", "nasal congestion" },
        { "Otolaryngology (ENT)", "hoarsnes", "hoarseness" },

        { "Ophthalmology", "blured vision", "blurred vision" },
        { "Ophthalmology", "doubl vision", "double vision" },
        { "Ophthalmology", "floters", "floaters" },
        { "Ophthalmology", "tunel vision", "tunnel vision" },
        { "Ophthalmology", "excesive tearing", "excessive tearing" },

        { "Psychiatry or Psychology", "anxity", "anxiety" },
        { "Psychiatry or Psychology", "depresion", "depression" },
        { "Psychiatry or Psychology", "panik attack", "panic attack" },
        { "Psychiatry or Psychology", "insomia", "insomnia" },
        { "Psychiatry or Psychology", "halucinations", "hallucinations" },

        { "Endocrinology", "diabets", "diabetes" },
        { "Endocrinology", "thyriod", "thyroid" },
        { "Endocrinology", "excesive thirst", "excessive thirst" },
        { "Endocrinology", "unexpcted weight gain", "unexpected weight gain" },
        { "Endocrinology", "low blod sugar", "low blood sugar" },

        { "Urology", "painful urintion", "painful urination" },
        { "Urology", "blod in urine", "blood in urine" },
        { "Urology", "frequent urintion", "frequent urination" },
        { "Urology", "cloudy urne", "cloudy urine" },
        { "Urology", "flank pan", "flank pain" },

        { "General Practice", "fevr", "fever" },
        { "General Practice", "fatige", "fatigue" },
        { "General Practice", "body akes", "body aches" },
        { "General Practice", "not feling well", "not feeling well" },
        { "General Practice", "medicl checkup", "medical checkup" }
    };
}
