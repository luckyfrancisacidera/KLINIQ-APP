using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Features.Practitioners.Mappings
{
    public static class PractitionerMappings
    {
        public static PractitonerDto ToDto(this Practitioner p) => new(
            p.Id, p.UserId,
            p.Name.FirstName, p.Name.LastName,
            p.LicenseNumber, p.Specialization,
            p.ClinicID
        );

        public static PractitionerDetailDto ToDetailDto(this Practitioner p) => new(
            p.Id, p.UserId,
            p.Name.FirstName, p.Name.LastName,
            p.LicenseNumber, p.Specialization,
            p.ClinicID,
            p.Schedules.Select(s => s.ToSummaryDto()).ToList()
        );

        public static ScheduleSummaryDto ToSummaryDto(this Schedule s) => new(
            s.Id,
            s.Day.ToString(),
            s.StartTime.ToString("HH:mm"),
            s.EndTime.ToString("HH:mm"),
            s.AppointmentLengthInMinutes,
            s.IsAvailable,
            s.Breaks.Select(b => new BreakDto(b.Id, b.StartTime.ToString("HH:mm"), b.EndTime.ToString("HH:mm"))).ToList()
        );
    }
}
