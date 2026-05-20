using Kliniq.Application.Features.Auth.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<AuthTokensInternal>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
