namespace Kliniq.Application.Features.Clinics.DTOs
{
    public sealed record ClinicPractitionerSummaryDto(
        Guid Id,
        string FirstName,
        string LastName,
        IReadOnlyList<string> Specializations);

    public sealed record ClinicSummaryDto(
        Guid Id,
        string Name,
        double Latitude,
        double Longitude,
        double? DistanceKm,
        int PractitionerCount,
        IReadOnlyList<string> Specializations);

    public sealed record ClinicDetailDto(
        Guid Id,
        string Name,
        double Latitude,
        double Longitude,
        IReadOnlyList<ClinicPractitionerSummaryDto> Practitioners);
}
