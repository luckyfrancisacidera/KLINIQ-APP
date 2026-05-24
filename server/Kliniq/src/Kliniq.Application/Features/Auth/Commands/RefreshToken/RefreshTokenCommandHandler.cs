using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Application.Features.Auth.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthTokensInternal>>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public RefreshTokenCommandHandler(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }
        
        public async Task<Result<AuthTokensInternal>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

            var user = authResult.Value!;

            if (!authResult.IsSuccess)
                return Result.Failure<AuthTokensInternal>(Error.Validation("Auth.InvalidRefreshToken", "Invalid or expired refresh token"));

            var accessToken = _jwtTokenService.GenerateAccessToken(user.UserId, user.Email, user.Role);

            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            var newRefreshTokenHash = _jwtTokenService.HashRefreshToken(newRefreshToken);

            await _authService.SaveRefreshTokenAsync(user.UserId, newRefreshTokenHash, cancellationToken);

            return Result.Success(new AuthTokensInternal
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                Response = new AuthResponseDto
                {
                    AccessTokenExpiresAtUtc = _jwtTokenService.GetAccessTokenExpiry(),
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                }
            });
        }

    }
}
