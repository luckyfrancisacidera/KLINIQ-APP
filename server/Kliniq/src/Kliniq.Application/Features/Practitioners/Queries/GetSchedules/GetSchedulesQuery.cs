using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetSchedules
{
    public sealed record GetSchedulesQuery(Guid PractitionerId) : IRequest<Result<IReadOnlyList<ScheduleSummaryDto>>>;
}
