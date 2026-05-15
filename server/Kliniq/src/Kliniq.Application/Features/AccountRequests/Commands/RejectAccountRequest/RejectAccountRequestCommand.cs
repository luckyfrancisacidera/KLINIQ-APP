using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest
{
    public class RejectAccountRequestCommand : IRequest<Result>
    {
        public Guid AccountRequestId { get; set; }
        public string AdminNote { get; set; } = string.Empty;
    }
}
