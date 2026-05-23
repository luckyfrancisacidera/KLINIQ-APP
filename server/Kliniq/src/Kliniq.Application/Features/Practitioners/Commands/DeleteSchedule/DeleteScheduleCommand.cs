using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.DeleteSchedule
{
    public sealed record DeleteScheduleCommand(Guid ScheduleId) : IRequest<Result>;
}
