using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Queries.GetAppointment
{
    public sealed class GetAppointmentQueryHandler : IRequestHandler<GetAppointmentQuery, Result<AppointmentDto>>
    {
        private readonly IAppointmentRepository _repository;
        public GetAppointmentQueryHandler(IAppointmentRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<AppointmentDto>> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _repository.GetByIdAsync(request.AppointmentId, cancellationToken);

            if (appointment is null)
                return Result.Failure<AppointmentDto>(Error.NotFound("Appointment.NotFound", $"Appointment '{request.AppointmentId}' was not found."));

            return Result.Success(appointment.ToDto());
        }
    }
}
