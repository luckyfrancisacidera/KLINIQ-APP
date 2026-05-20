using Kliniq.Application.Features.Auth.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthTokensInternal>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
