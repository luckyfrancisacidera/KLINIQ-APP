using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Queries.GetAppointment
{
    public sealed record GetAppointmentQuery(Guid AppointmentId) : IRequest<Result<AppointmentDto>>;

}
