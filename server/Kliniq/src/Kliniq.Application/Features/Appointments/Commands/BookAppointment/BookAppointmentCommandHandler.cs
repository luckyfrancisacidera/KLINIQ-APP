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
            var patientExists = await _patientRepository.ExistByIdAsync(request.PatientId, cancellationToken);

            if(!patientExists)
                return Result.Failure<AppointmentDto>(Error.NotFound("Patient.NotFound", $"Patient '{request.PatientId}' not found."));
            
            var practitionerExists = await _practitionerRepository.ExistsAsync(request.PractitionerId, cancellationToken);

            if(!practitionerExists)
                return Result.Failure<AppointmentDto>(Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}' not found."));

            var dayOfWeek = request.ScheduledAt.DayOfWeek;
            var clinicDay = dayOfWeek == DayOfWeek.Sunday ? ClinicDayOfWeek.Sunday : (ClinicDayOfWeek)(int)dayOfWeek;

            var schedules = await _scheduleRepository.GetByPractitionerIdAsync(request.PractitionerId, cancellationToken);

            var requestedTime = TimeOnly.FromDateTime(request.ScheduledAt);

            var schedule = schedules.FirstOrDefault(s => s.Day == clinicDay && s.IsAvailable && s.CoversTimeSlot(requestedTime, requestedTime.AddMinutes(s.AppointmentLengthMinutes)));

            if (schedule == null)
                return Result.Failure<AppointmentDto>(Error.Validation("Schedule.Unavailable", $"No available schedule for the requested time '{request.ScheduledAt}' with practitioner '{request.PractitionerId}'."));
            
            bool hasConflict = await _appointmentRepository.HasConflictAsync(request.PractitionerId, request.ScheduledAt,schedule.AppointmentLengthMinutes, excludeId: null, cancellationToken);

            if(hasConflict)
                return Result.Failure<AppointmentDto>(Error.Conflict("Appointment.SlotTaken", $"The slot at {requestedTime:HH:mm} is already booked."));

            var appointment = new Appointment(
                request.PatientId,
                request.PractitionerId,
                request.ClinicId,
                request.ScheduledAt,
                TimeSpan.FromMinutes(schedule.AppointmentLengthMinutes),
                request.Reason);

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(appointment.ToDto());
        }

    }
}
