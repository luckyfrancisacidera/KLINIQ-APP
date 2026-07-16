using Kliniq.Api.Extensions;
using Kliniq.Application.Features.Auth.Commands.ChangePassword;
using Kliniq.Application.Features.Auth.Commands.ForgotPassword;
using Kliniq.Application.Features.Auth.Commands.Login;
using Kliniq.Application.Features.Auth.Commands.ResetPassword;
using Kliniq.Application.Features.Auth.Commands.RefreshToken;
using Kliniq.Application.Features.Auth.Commands.Register;
using Kliniq.Application.Features.Auth.Commands.RevokeToken;
using Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword;
using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Kliniq.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public sealed class AuthController : ControllerBase
    {
        private const string AccessTokenCookie = "accessToken";
        private const string RefreshTokenCookie = "refreshToken";

        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }

        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ToActionResult();
            SetAuthCookies(result.Value!);
            return Ok(BuildResponse(result.Value!));
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ToActionResult();
            SetAuthCookies(result.Value!);
            return Ok(BuildResponse(result.Value!));
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[RefreshTokenCookie];
            if (string.IsNullOrWhiteSpace(refreshToken)) return Unauthorized();

            var result = await _mediator.Send(new RefreshTokenCommand { RefreshToken = refreshToken }, cancellationToken);
            if (!result.IsSuccess)
            {
                ClearAuthCookies();
                return result.ToActionResult();
            }

            SetAuthCookies(result.Value!);
            return Ok(BuildResponse(result.Value!));
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (userId is null) return Unauthorized();
            var result = await _mediator.Send(new RevokeTokenCommand { UserId = userId }, cancellationToken);
            ClearAuthCookies();
            if (result.IsFailure) return result.ToActionResult();
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId is null) return Unauthorized();

            return Ok(new AuthResponseDto
            {
                UserId = userId,
                Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return Accepted(new { message = "If an account exists for that email, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return NoContent();
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId is null) return Unauthorized();

            var result = await _mediator.Send(command with { UserId = userId }, cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            ClearAuthCookies();
            return NoContent();
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

        private CookieOptions CreateCookieOptions(DateTime expiresAtUtc, string path, SameSiteMode sameSite) => new()
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment() || Request.IsHttps,
            SameSite = sameSite,
            Expires = expiresAtUtc,
            IsEssential = true,
            Path = path
        };

        private void SetAuthCookies(AuthTokensInternal tokens)
        {
            Response.Cookies.Append(
                AccessTokenCookie,
                tokens.AccessToken,
                CreateCookieOptions(tokens.Response.AccessTokenExpiresAtUtc, "/", SameSiteMode.Lax));

            Response.Cookies.Append(
                RefreshTokenCookie,
                tokens.RefreshToken,
                CreateCookieOptions(DateTime.UtcNow.AddDays(7), "/api/auth/", SameSiteMode.Strict));
        }

        private void ClearAuthCookies()
        {
            var accessOptions = CreateCookieOptions(DateTime.UnixEpoch, "/", SameSiteMode.Lax);
            var refreshOptions = CreateCookieOptions(DateTime.UnixEpoch, "/api/auth/", SameSiteMode.Strict);
            Response.Cookies.Delete(AccessTokenCookie, accessOptions);
            Response.Cookies.Delete(RefreshTokenCookie, refreshOptions);
        }

        private static AuthResponseDto BuildResponse(AuthTokensInternal tokens) => tokens.Response;
    }
}
