using Kliniq.Application.Common.Interfaces;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result>
    {
        private readonly IAuthService _authService;

        public RevokeTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            await _authService.RevokeTokenAsync(request.UserId, cancellationToken);
            return Result.Success();
        }
    }
}
