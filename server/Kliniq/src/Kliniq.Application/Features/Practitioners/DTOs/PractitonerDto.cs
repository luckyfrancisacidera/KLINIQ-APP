namespace Kliniq.Application.Features.Practitioners.DTOs
{
    public sealed record PractitonerDto(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string LicenseNumber,
        string Specialization,
        Guid? ClinicId
    );

    public sealed record PractitionerDetailDto(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string LicenseNumber,
        string Specialization,
        Guid? ClinicId,
        IReadOnlyList<ScheduleSummaryDto> Schedules
    );

    public sealed record ScheduleSummaryDto(
        Guid Id,
        string Day,
        string StartTime,
        string EndTime,
        int AppointmentDurationMinutes,
        bool isAvailable,
        IReadOnlyList<BreakDto> Breaks
     );

    public sealed record BreakDto(
        Guid Id,
        string StartTime,
        string EndTime
    );

    public sealed record AvailableSlotDto(
        string Day,
        IReadOnlyList<string> Slots
    );
}
