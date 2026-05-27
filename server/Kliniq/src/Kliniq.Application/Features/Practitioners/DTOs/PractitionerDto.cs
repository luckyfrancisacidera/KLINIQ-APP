namespace Kliniq.Application.Features.Practitioners.DTOs
{
    public sealed record ClinicSummaryDto(
        Guid Id,
        string Name,
        double Latitude,
        double Longitude
    );

    public sealed record PractitionerDto(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string LicenseNumber,
        IReadOnlyList<string> Specializations,
        Guid? ClinicId,
        ClinicSummaryDto? Clinic
    );

    public sealed record PractitionerDetailDto(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string LicenseNumber,
        IReadOnlyList<string> Specializations,
        Guid? ClinicId,
        ClinicSummaryDto? Clinic,
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
        Guid ScheduleId,       
        DateOnly Date,        
        string DayOfWeek,      
        IReadOnlyList<string> Slots 
    );
}
