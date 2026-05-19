using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommand : IRequest<Result>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
