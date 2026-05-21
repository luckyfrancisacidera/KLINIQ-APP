using Kliniq.Api.Extensions;
using Kliniq.Application.Features.Auth.Commands.Login;
using Kliniq.Application.Features.Auth.Commands.RefreshToken;
using Kliniq.Application.Features.Auth.Commands.Register;
using Kliniq.Application.Features.Auth.Commands.RevokeToken;
using Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword;
using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Kliniq.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string AccessTokenCookie = "accessToken";
        private const string RefreshTokenCookie = "refreshToken";

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private static CookieOptions BaseCookieOptions() => new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
        };

        private static CookieOptions AccessTokenCookieOptions(DateTime expiresAat)
        {
            var options = BaseCookieOptions();
            options.Expires = expiresAat;
            options.Path ="/";
            return options;
        }

        private static CookieOptions RefreshTokenCookieOptions()
        {
            var options = BaseCookieOptions();
            options.Expires = DateTime.UtcNow.AddDays(7);
            options.Path = "/api/auth/";
            return options;
        }

        private void SetAuthCookies(AuthTokensInternal tokens)
        {
            Response.Cookies.Append(AccessTokenCookie, tokens.AccessToken, AccessTokenCookieOptions(tokens.Response.AccessTokenExpiresAtUtc));  
            Response.Cookies.Append(RefreshTokenCookie, tokens.RefreshToken, RefreshTokenCookieOptions());
        }

        private void ClearAuthCookies()
        {
            Response.Cookies.Delete(AccessTokenCookie, new CookieOptions { Path = "/" });
            Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions { Path = "/api/auth/" });
        }

        //ENDPOINTS
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ToActionResult();

            var tokens = result.Value!;

            SetAuthCookies(tokens);

        #if DEBUG
            var responseWithToken = tokens.Response with { DevAccessToken = tokens.AccessToken };
            return Ok(responseWithToken);
        #else
            return Ok(tokens.Response);
        #endif
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if(!result.IsSuccess) return result.ToActionResult();

            var tokens = result.Value!;

            SetAuthCookies(tokens);

        #if DEBUG
            var responseWithToken = tokens.Response with { DevAccessToken = tokens.AccessToken };
            return Ok(responseWithToken);
        #else
            return Ok(tokens.Response);
        #endif
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[RefreshTokenCookie];

            if(string.IsNullOrEmpty(refreshToken)) return Unauthorized();

            var result = await _mediator.Send(new RefreshTokenCommand { RefreshToken = refreshToken }, cancellationToken);

            var tokens = result.Value!;

            SetAuthCookies(tokens);

        #if DEBUG
            var responseWithToken = tokens.Response with { DevAccessToken = tokens.AccessToken };
            return Ok(responseWithToken);
        #else
            return Ok(tokens.Response);
        #endif

        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var result  = await _mediator.Send(new RevokeTokenCommand { UserId = userId }, cancellationToken);

            ClearAuthCookies();
            return result.ToActionResult();
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if(userId is null) return Unauthorized();

            return Ok(new AuthResponseDto
            {
                UserId = userId,
                Email = email ?? string.Empty,
                Role = role ?? string.Empty
            });
        }

        [HttpPost("set-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> SetPassword([FromBody] SetPractitionerPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
