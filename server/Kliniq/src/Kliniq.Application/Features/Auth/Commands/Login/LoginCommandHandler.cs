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

            if (!authResult.IsSuccess)
                return Result.Failure<AuthTokensInternal>(authResult.Error!);

            var accessToken = _jwtTokenService.GenerateAccessToken(authResult.Value!.UserId, authResult.Value.Email, authResult.Value.Role);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenHash = _jwtTokenService.HashRefreshToken(refreshToken);

            var saveResult = await _authService.SaveRefreshTokenAsync(authResult.Value!.UserId, refreshTokenHash, cancellationToken);

            if(!saveResult.IsSuccess)
                return Result.Failure<AuthTokensInternal>(saveResult.Error!);

            return Result.Success(new AuthTokensInternal
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken, 
                Response = new AuthResponseDto
                {
                    AccessTokenExpiresAtUtc = _jwtTokenService.GetAccessTokenExpiry(),
                    UserId = authResult.Value.UserId,
                    Email = authResult.Value.Email,
                    Role = authResult.Value.Role
                }
            });
        }
    }
}
                                    