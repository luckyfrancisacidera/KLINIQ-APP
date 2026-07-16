using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Queries.GetAccountRequest;

public sealed record GetAccountRequestQuery(Guid Id) : IRequest<Result<AccountRequestDto>>;
