using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.CancelAppointment
{
    public sealed record CancelAppointmentCommand(Guid AppointmentId) : IRequest<Result<AppointmentDto>>;

    public sealed class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result<AppointmentDto>>
    {
        private readonly IAppointmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentCommandHandler(IAppointmentRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AppointmentDto>> Handle(
            CancelAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            var appointment = await _repository.GetByIdTrackedAsync(request.AppointmentId, cancellationToken);

            if (appointment is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Appointment.NotFound", $"Appointment '{request.AppointmentId}' was not found."));

            appointment.Cancel();

            _repository.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(appointment.ToDto());
        }
    }
}