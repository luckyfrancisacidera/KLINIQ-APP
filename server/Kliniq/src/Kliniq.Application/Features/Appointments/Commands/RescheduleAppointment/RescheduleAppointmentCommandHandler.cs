using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.RescheduleAppointment
{
    public sealed class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result<AppointmentDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppTimeZone _appTimeZone;

        public RescheduleAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork,
            IAppTimeZone appTimeZone)
        {
            _appointmentRepository = appointmentRepository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _appTimeZone = appTimeZone;
        }

        public async Task<Result<AppointmentDto>> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdTrackedAsync(request.AppointmentId, cancellationToken);
            if (appointment is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Appointment.NotFound", "Appointment was not found."));

            var schedule = await _scheduleRepository.GetByIdWithBreaksAsync(request.ScheduleId, cancellationToken);
            if (schedule is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Schedule.NotFound", "Schedule was not found."));

            if (schedule.PractitionerId != appointment.PractitionerId)
                return Result.Failure<AppointmentDto>(Error.Validation("Appointment.PractitionerMismatch", "Rescheduling must use the same practitioner."));

            var expectedDay = schedule.Day == ClinicDayOfWeek.Sunday
                ? DayOfWeek.Sunday
                : (DayOfWeek)(int)schedule.Day;

            if (request.AppointmentDate.DayOfWeek != expectedDay)
                return Result.Failure<AppointmentDto>(Error.Validation("Schedule.DayMismatch", "The selected date does not match the schedule day."));

            var slotEnd = request.SlotTime.AddMinutes(schedule.AppointmentLengthMinutes);
            if (!schedule.CoversTimeSlot(request.SlotTime, slotEnd))
                return Result.Failure<AppointmentDto>(Error.Validation("Schedule.SlotInvalid", "The selected time is not available in this schedule."));

            var scheduledAt = _appTimeZone.ToUtc(request.AppointmentDate, request.SlotTime);
            var hasConflict = await _appointmentRepository.HasConflictAsync(
                appointment.PractitionerId,
                scheduledAt,
                schedule.AppointmentLengthMinutes,
                appointment.Id,
                cancellationToken);

            if (hasConflict)
                return Result.Failure<AppointmentDto>(Error.Conflict("Appointment.SlotTaken", "The selected slot is no longer available."));

            appointment.Reschedule(scheduledAt, TimeSpan.FromMinutes(schedule.AppointmentLengthMinutes));
            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(appointment.ToDto());
        }
    }
}
