using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetAvailableSlots
{
    public sealed class GetAvailableSlotsQueryHandler
        : IRequestHandler<GetAvailableSlotsQuery, Result<IReadOnlyList<AvailableSlotDto>>>
    {
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAppTimeZone _appTimeZone;

        public GetAvailableSlotsQueryHandler(
            IPractitionerRepository practitionerRepository,
            IScheduleRepository scheduleRepository,
            IAppointmentRepository appointmentRepository,
            IAppTimeZone appTimeZone)
        {
            _practitionerRepository = practitionerRepository;
            _scheduleRepository = scheduleRepository;
            _appointmentRepository = appointmentRepository;
            _appTimeZone = appTimeZone;
        }

        public async Task<Result<IReadOnlyList<AvailableSlotDto>>> Handle(
            GetAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            var today = _appTimeZone.Today;
            var resolvedFrom = request.From ?? today;
            var resolvedTo = request.To ?? resolvedFrom.AddDays(7);

            if (resolvedFrom < today)
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(
                    Error.Validation("Slots.PastDate", "From date cannot be in the past."));

            if (resolvedTo < resolvedFrom)
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(
                    Error.Validation("Slots.InvalidRange", "To date must be on or after From date."));

            if (resolvedTo > resolvedFrom.AddDays(60))
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(
                    Error.Validation("Slots.RangeTooWide", "Date range cannot exceed 60 days."));

            var practitionerExists = await _practitionerRepository
                .ExistsAsync(request.PractitionerId, cancellationToken);

            if (!practitionerExists)
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(
                    Error.NotFound("Practitioner.NotFound",
                        $"Practitioner '{request.PractitionerId}' was not found."));

            var schedules = await _scheduleRepository
                .GetByPractitionerIdAsync(request.PractitionerId, cancellationToken);

            var availableSchedules = schedules
                .Where(s => s.IsAvailable)
                .ToList();

            var fromUtc = _appTimeZone.ToUtc(resolvedFrom, TimeOnly.MinValue);
            var toUtc = _appTimeZone.ToUtc(resolvedTo, TimeOnly.MaxValue);

            var existingAppointments = await _appointmentRepository
                .GetByPractitionerInRangeAsync(request.PractitionerId, fromUtc, toUtc, cancellationToken);

            var bookedByDate = existingAppointments
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .GroupBy(a => _appTimeZone.ToLocalDate(a.ScheduledAt))
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(appointment => _appTimeZone.ToLocalTime(appointment.ScheduledAt)).ToHashSet());

            var result = new List<AvailableSlotDto>();

            for (var date = resolvedFrom; date <= resolvedTo; date = date.AddDays(1))
            {
                var clinicDay = ToClinicDayOfWeek(date.DayOfWeek);
                var schedule = availableSchedules.FirstOrDefault(s => s.Day == clinicDay);

                if (schedule is null) continue; 

                var bookedSlots = bookedByDate.GetValueOrDefault(date) ?? [];

                var currentLocalTime = TimeOnly.FromDateTime(_appTimeZone.LocalNow);
                var freeSlots = schedule
                    .GetAvailableSlots()
                    .Where(slot => date > today || slot > currentLocalTime)
                    .Where(slot => !bookedSlots.Contains(slot))
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

        private static ClinicDayOfWeek ToClinicDayOfWeek(DayOfWeek day)
            => day == DayOfWeek.Sunday ? ClinicDayOfWeek.Sunday : (ClinicDayOfWeek)(int)day;
    }
}