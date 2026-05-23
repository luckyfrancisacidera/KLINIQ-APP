using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.AddScheduleBreak
{
    public sealed record AddScheduleBreakCommand(
        Guid ScheduleId,
        string BreakStart,
        string BreakEnd
        ) : IRequest<Result<ScheduleSummaryDto>>;
}
