using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest
{
    public class RejectAccountRequestCommand : IRequest<bool>
    {
        public Guid AccountRequestId { get; set; }
        public string AdminNote { get; set; } = string.Empty;
    }
}
