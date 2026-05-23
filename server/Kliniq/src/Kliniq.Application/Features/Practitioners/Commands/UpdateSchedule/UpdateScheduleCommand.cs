using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.UpdateSchedule
{
    public sealed record UpdateScheduleCommand(
        Guid ScheduleId,
        string Day,
        string StartTime,
        string EndTime,
        int AppointmentLengthMinutes
    ) : IRequest<Result<ScheduleSummaryDto>>;
}
