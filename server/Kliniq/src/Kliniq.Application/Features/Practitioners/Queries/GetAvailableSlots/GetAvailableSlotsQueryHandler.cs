using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetAvailableSlots
{
    public sealed class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, Result<IReadOnlyList<AvailableSlotDto>>>
    {
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        public GetAvailableSlotsQueryHandler(IPractitionerRepository practitionerRepository, IScheduleRepository scheduleRepository, IAppointmentRepository appointmentRepository)
        {
            _practitionerRepository = practitionerRepository;
            _scheduleRepository = scheduleRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<IReadOnlyList<AvailableSlotDto>>> Handle( GetAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            if (request.To < request.From)
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(
                    Error.Validation("Slots.InvalidRange", "To date must be on or after From date."));

            if (request.To > request.From.AddDays(60))
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(
                    Error.Validation("Slots.RangeTooWide", "Date range cannot exceed 60 days."));

            var practitionerExists = await _practitionerRepository.ExistsAsync(request.PractitionerId, cancellationToken);
            if (!practitionerExists)
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(
                    Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}' not found."));

            var schedules = await _scheduleRepository.GetByPractitionerIdAsync(request.PractitionerId, cancellationToken);
            var availableSchedules = schedules.Where(s => s.IsAvailable).ToList();

            var fromUtc = request.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var toUtc = request.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            var existingAppointments = await _appointmentRepository.GetByPractitionerInRangeAsync(request.PractitionerId, fromUtc, toUtc, cancellationToken);

            var result = new List<AvailableSlotDto>();

            for (var date = request.From; date <= request.To; date = date.AddDays(1))
            {
                var dotNetDay = date.DayOfWeek;
                var clinicDay = dotNetDay == DayOfWeek.Sunday
                    ? ClinicDayOfWeek.Sunday
                    : (ClinicDayOfWeek)(int)dotNetDay;

                var schedule = availableSchedules.FirstOrDefault(s => s.Day == clinicDay);
                if (schedule is null) continue;

                var allSlots = schedule.GetAvailableSlots();

                var bookedStartTimes = existingAppointments
                    .Where(a =>
                        a.Status != AppointmentStatus.Cancelled &&
                        DateOnly.FromDateTime(a.ScheduledAt) == date)
                    .Select(a => TimeOnly.FromDateTime(a.ScheduledAt))
                    .ToHashSet();

                var freeSlots = allSlots
                    .Where(slot => !bookedStartTimes.Contains(slot))
                    .Select(slot => slot.ToString("HH:mm"))
                    .ToList();

                if (freeSlots.Count == 0) continue;

                result.Add(new AvailableSlotDto(
                    schedule.Id,
                    date,
                    date.DayOfWeek.ToString(),
                    freeSlots));
            }

            return Result.Success<IReadOnlyList<AvailableSlotDto>>(result);
        }

    }
}
