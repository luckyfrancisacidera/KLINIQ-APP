using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Queries.GetPractitionerAppointments
{
    public sealed record GetPractitionerAppointmentsQuery(
        Guid PractitionerId,
        string? Status = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<Result<PagedResult<AppointmentDto>>>;
}
