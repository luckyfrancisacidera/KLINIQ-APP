using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Application.Features.AccountRequests.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Queries.GetAccountRequest;

public sealed class GetAccountRequestQueryHandler : IRequestHandler<GetAccountRequestQuery, Result<AccountRequestDto>>
{
    private readonly IAccountRequestRepository _repository;

    public GetAccountRequestQueryHandler(IAccountRequestRepository repository) => _repository = repository;

    public async Task<Result<AccountRequestDto>> Handle(GetAccountRequestQuery request, CancellationToken cancellationToken)
    {
        var accountRequest = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return accountRequest is null
            ? Result.Failure<AccountRequestDto>(Error.NotFound("AccountRequest.NotFound", "Account request not found"))
            : Result.Success(accountRequest.ToDto());
    }
}
