using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.BookAppointment
{
    public sealed class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, Result<AppointmentDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BookAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository, 
            IPatientRepository patientRepository,
            IPractitionerRepository practitionerRepository,
            IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _practitionerRepository = practitionerRepository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AppointmentDto>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdWithBreaksAsync(request.ScheduleId, cancellationToken);

            if (schedule is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Schedule.NotFound", $"Schedule '{request.ScheduleId}' not found."));

            if (!schedule.IsAvailable)
                return Result.Failure<AppointmentDto>(Error.Validation("Schedule.Unavailable", "This schedule is not available."));

            var slotEnd = request.SlotTime.AddMinutes(schedule.AppointmentLengthMinutes);
            if (!schedule.CoversTimeSlot(request.SlotTime, slotEnd))
                return Result.Failure<AppointmentDto>(Error.Validation("Schedule.SlotInvalid", $"The slot '{request.SlotTime:HH:mm}' is not valid for this schedule."));

            var practitioner = await _practitionerRepository.GetByIdAsync(schedule.PractitionerId, cancellationToken);
            if (practitioner is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Practitioner.NotFound", "Practitioner not found."));

            if (practitioner.ClinicID is null)
                return Result.Failure<AppointmentDto>(Error.Validation("Practitioner.NoClinic", "Practitioner is not assigned to a clinic."));

            var patient = await _patientRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (patient is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Patient.NotFound", "Patient profile not found."));

            var expectedDotNetDay = schedule.Day == ClinicDayOfWeek.Sunday
                ? DayOfWeek.Sunday
                : (DayOfWeek)(int)schedule.Day;

            if (request.AppointmentDate.DayOfWeek != expectedDotNetDay)
                return Result.Failure<AppointmentDto>(Error.Validation(
                    "Schedule.DayMismatch",
                    $"The date '{request.AppointmentDate}' is a {request.AppointmentDate.DayOfWeek}, but this schedule runs on {schedule.Day}."));

            var scheduledAt = request.AppointmentDate.ToDateTime(request.SlotTime, DateTimeKind.Utc);

            bool hasConflict = await _appointmentRepository.HasConflictAsync(
                schedule.PractitionerId, scheduledAt, schedule.AppointmentLengthMinutes, excludeId: null, cancellationToken);

            if (hasConflict)
                return Result.Failure<AppointmentDto>(Error.Conflict("Appointment.SlotTaken", $"The slot at {request.SlotTime:HH:mm} is already booked."));

            var appointment = new Appointment(
                patient.Id,
                schedule.PractitionerId,
                practitioner.ClinicID.Value,
                scheduledAt,
                TimeSpan.FromMinutes(schedule.AppointmentLengthMinutes),
                request.Reason);

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(appointment.ToDto());
        }

    }
}
