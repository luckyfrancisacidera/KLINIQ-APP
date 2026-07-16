using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetCurrentPractitioner
{
    public sealed record GetCurrentPractitionerQuery(Guid UserId) : IRequest<Result<PractitionerDetailDto>>;
}
