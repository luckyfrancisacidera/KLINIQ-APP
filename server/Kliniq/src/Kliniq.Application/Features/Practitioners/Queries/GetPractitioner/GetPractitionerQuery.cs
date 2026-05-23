using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetPractitioner
{
    public sealed record GetPractitionerQuery(Guid PractitionerId) : IRequest<Result<PractitionerDetailDto>>;
}
