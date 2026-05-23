using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetPractitioners
{
    public sealed class GetPractitionerQueryHandler : IRequestHandler<GetPractitionersQuery, Result<PagedResult<PractitionerDto>>>
    {
        private readonly IPractitionerRepository _repository;

        public GetPractitionerQueryHandler(IPractitionerRepository repository) => _repository = repository;

        public async Task<Result<PagedResult<PractitionerDto>>> Handle(GetPractitionersQuery request, CancellationToken cancellationToken)
        {
            var paged = await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

            var mapped = new PagedResult<PractitionerDto>(
                paged.Items.Select(p => p.ToDto()).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize
            );

            return Result.Success(mapped);

        }
    }
}
