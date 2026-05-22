using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.CreateSchedule
{
    public sealed record CreateScheduleCommand(
        Guid PractitionerId,
        string Day,
        string StartTime,
        string EndTime,
        int AppointmentLengthMinutes
    ) : IRequest<Result<ScheduleSummaryDto>>;
}
