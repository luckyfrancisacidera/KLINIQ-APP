using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Features.Practitioners.Mappings
{
    public static class PractitionerMappings
    {
        private static IReadOnlyList<string> ParseSpecializations(string raw) =>
            raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
               .Select(s => s.Trim('[', ']', '"', '\'', ' '))
               .Where(s => !string.IsNullOrWhiteSpace(s))
               .ToList()
               .AsReadOnly();

        private static ClinicSummaryDto? MapClinic(Clinic? clinic) =>
            clinic is null ? null : new ClinicSummaryDto(
                clinic.Id,
                clinic.Name,
                clinic.Location.Latitude,
                clinic.Location.Longitude
            );

        public static PractitionerDto ToDto(this Practitioner p) => new(
            p.Id,
            p.UserId,
            p.Name.FirstName,
            p.Name.LastName,
            p.LicenseNumber,
            ParseSpecializations(p.SpecializationsRaw),
            p.ClinicID,
            MapClinic(p.Clinic)
        );

        public static PractitionerDetailDto ToDetailDto(this Practitioner p) => new(
            p.Id,
            p.UserId,
            p.Name.FirstName,
            p.Name.LastName,
            p.LicenseNumber,
            ParseSpecializations(p.SpecializationsRaw),
            p.ClinicID,
            MapClinic(p.Clinic),
            p.Schedules.Select(s => s.ToSummaryDto()).ToList()
        );

        public static ScheduleSummaryDto ToSummaryDto(this Schedule s) => new(
            s.Id,
            s.Day.ToString(),
            s.StartTime.ToString("HH:mm"),
            s.EndTime.ToString("HH:mm"),
            s.AppointmentLengthMinutes,
            s.IsAvailable,
            s.Breaks.Select(b => new BreakDto(b.Id, b.StartTime.ToString("HH:mm"), b.EndTime.ToString("HH:mm"))).ToList()
        );
    }
}