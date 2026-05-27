using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Features.Appointments.Mappings
{
    public static class AppointmentMappings
    {
        public static AppointmentDto ToDto(this Appointment a) => new(
            a.Id,
            a.PatientId,
            a.PractitionerId,
            a.ClinicId,
            a.ScheduledAt,
            (int)a.Duration.TotalMinutes,
            a.Status.ToString(),
            a.Reason,
            a.Notes
        );
    }
}
