using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
