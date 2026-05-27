using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.CompleteAppointment
{
    public sealed record CompleteAppointmentCommand(Guid AppointmentId, string? Notes = null) : IRequest<Result<AppointmentDto>>;

    public sealed class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, Result<AppointmentDto>>
    {
        private readonly IAppointmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteAppointmentCommandHandler(IAppointmentRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AppointmentDto>> Handle(
            CompleteAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            var appointment = await _repository
                .GetByIdTrackedAsync(request.AppointmentId, cancellationToken);

            if (appointment is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Appointment.NotFound", $"Appointment '{request.AppointmentId}' was not found."));

            appointment.Complete(request.Notes);

            _repository.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(appointment.ToDto());
        }
    }
}