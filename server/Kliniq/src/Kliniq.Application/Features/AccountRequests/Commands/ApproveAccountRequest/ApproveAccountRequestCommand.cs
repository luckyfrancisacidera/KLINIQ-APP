using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest
{
    public class ApproveAccountRequestCommand : IRequest<Result>
    {
        public Guid AccountRequestId { get; set; }
        public string? AdminNote { get; set; }
    }
}
