using Kliniq.Domain.Common;

namespace Kliniq.Application.Features.SymptomSearch.DTOs
{
    public sealed record SpecialtySuggestionDto(
        string Specialty,
        int MatchScore,
        IReadOnlyList<string> MatchedSignals);

    public sealed record SuggestedPractitionerDto(
        Guid Id,
        string FirstName,
        string LastName,
        string LicenseNumber,
        IReadOnlyList<string> Specializations,
        Guid? ClinicId,
        string? ClinicName,
        int MatchScore);

    public sealed record SymptomSearchResponseDto(
        string Urgency,
        string Guidance,
        IReadOnlyList<SpecialtySuggestionDto> SuggestedSpecialties,
        PagedResult<SuggestedPractitionerDto> Practitioners,
        string Disclaimer);
}
