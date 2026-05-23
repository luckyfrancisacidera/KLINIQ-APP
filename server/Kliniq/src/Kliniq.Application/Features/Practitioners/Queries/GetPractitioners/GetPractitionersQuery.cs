using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetPractitioners
{
    public sealed record GetPractitionersQuery(
        int Page = 1,
        int PageSize = 20
    ) : IRequest<Result<PagedResult<PractitionerDto>>>;
}
