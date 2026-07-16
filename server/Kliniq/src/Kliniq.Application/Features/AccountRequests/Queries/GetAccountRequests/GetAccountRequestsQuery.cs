using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Queries.GetAccountRequests;

public sealed record GetAccountRequestsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Status = null) : IRequest<Result<PagedResult<AccountRequestSummaryDto>>>;
