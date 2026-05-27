using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetAvailableSlots
{
    public sealed record GetAvailableSlotsQuery(
        Guid PractitionerId,
        DateOnly From,
        DateOnly To
    ) : IRequest<Result<IReadOnlyList<AvailableSlotDto>>>;
}
