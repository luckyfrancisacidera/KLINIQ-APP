using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Queries.GetPatientAppointments
{
    public sealed record GetPatientAppointmentsQuery(
        Guid PatientId,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<Result<PagedResult<AppointmentDto>>>;
}
