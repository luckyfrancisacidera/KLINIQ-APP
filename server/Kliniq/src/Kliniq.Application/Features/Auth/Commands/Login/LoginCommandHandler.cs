using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Domain.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Kliniq.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;     
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

            var accessToken = _jwtTokenService.GenerateAccessToken(authResult.UserId, authResult.Email, authResult.Role);

            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenHash = _jwtTokenService.HashRefreshToken(refreshToken);

            await _authService.SaveRefreshTokenAsync(
                authResult.UserId,
                refreshTokenHash,   
                cancellationToken);

            return Result.Success(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,    
                AccessTokenExpiresAtUtc = _jwtTokenService.GetAccessTokenExpiry(),
                UserId = authResult.UserId,
                Email = authResult.Email,
                Role = authResult.Role
            });
        }
    }
}
