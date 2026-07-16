using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.AccountRequests.Mappings;
using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Queries.GetAccountRequests;

public sealed class GetAccountRequestsQueryHandler
    : IRequestHandler<GetAccountRequestsQuery, Result<PagedResult<AccountRequestSummaryDto>>>
{
    private readonly IAccountRequestRepository _repository;

    public GetAccountRequestsQueryHandler(IAccountRequestRepository repository) => _repository = repository;

    public async Task<Result<PagedResult<AccountRequestSummaryDto>>> Handle(
        GetAccountRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var paged = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.Status,
            cancellationToken);

        return Result.Success(new PagedResult<AccountRequestSummaryDto>(
            paged.Items.Select(item => item.ToSummaryDto()).ToList(),
            paged.TotalItems,
            paged.Page,
            paged.PageSize));
    }
}
