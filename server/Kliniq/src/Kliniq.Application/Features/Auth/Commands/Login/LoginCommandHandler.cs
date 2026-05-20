using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Application.Features.Auth.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokensInternal>>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;     
        }

        public async Task<Result<AuthTokensInternal>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

            var accessToken = _jwtTokenService.GenerateAccessToken(authResult.UserId, authResult.Email, authResult.Role);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenHash = _jwtTokenService.HashRefreshToken(refreshToken);

            await _authService.SaveRefreshTokenAsync(authResult.UserId, refreshTokenHash, cancellationToken);

            return Result.Success(new AuthTokensInternal
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken, 
                Response = new AuthResponseDto
                {
                    AccessTokenExpiresAtUtc = _jwtTokenService.GetAccessTokenExpiry(),
                    UserId = authResult.UserId,
                    Email = authResult.Email,
                    Role = authResult.Role
                }
            });
        }
    }
}
                                    