using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.QueueAppointment
{
    public sealed record QueueAppointmentCommand(Guid AppointmentId) : IRequest<Result<AppointmentDto>>;

    public sealed class QueueAppointmentCommandHandler : IRequestHandler<QueueAppointmentCommand, Result<AppointmentDto>>
    {
        private readonly IAppointmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppTimeZone _appTimeZone;

        public QueueAppointmentCommandHandler(
            IAppointmentRepository repository,
            IUnitOfWork unitOfWork,
            IAppTimeZone appTimeZone)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _appTimeZone = appTimeZone;
        }

        public async Task<Result<AppointmentDto>> Handle(QueueAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _repository.GetByIdTrackedAsync(request.AppointmentId, cancellationToken);
            if (appointment is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Appointment.NotFound", "Appointment was not found."));

            if (_appTimeZone.ToLocalDate(appointment.ScheduledAt) != _appTimeZone.Today)
                return Result.Failure<AppointmentDto>(Error.Validation(
                    "Appointment.QueueDateInvalid",
                    "Patients can only be placed in the queue on the scheduled appointment date."));

            appointment.JoinQueue(DateTime.UtcNow);
            _repository.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(appointment.ToDto());
        }
    }
}
