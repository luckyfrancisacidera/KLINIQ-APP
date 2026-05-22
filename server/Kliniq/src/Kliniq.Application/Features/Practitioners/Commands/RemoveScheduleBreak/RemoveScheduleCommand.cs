using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.RemoveScheduleBreak
{
    public sealed record RemoveScheduleCommand(
        Guid ScheduleId,
        Guid BreakId
    ) : IRequest<Result<ScheduleSummaryDto>>;
}
