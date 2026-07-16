namespace Kliniq.Application.Features.Appointments.DTOs
{
    public sealed record AppointmentDto(
        Guid Id,
        Guid PatientId,
        Guid PractitionerId,
        Guid ClinicId,
        DateTime ScheduledAt,
        int DurationMinutes,
        string Status,
        string? Reason,
        string? Notes,
        DateTime? QueuedAtUtc,
        DateTime? ConsultationStartedAtUtc,
        DateTime? CompletedAtUtc
    );
}
