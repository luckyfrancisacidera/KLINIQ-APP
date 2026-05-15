using Kliniq.Api.Extensions;
using Kliniq.Api.Models.Requests;
using Kliniq.Application.Features.Auth.Commands.Login;
using Kliniq.Application.Features.Auth.Commands.Register;
using Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword;
using Kliniq.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(RefreshTokenCommand, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;

            if(userId is null)
                return Unauthorized();

            var result  = await _mediator.Send(new RevokeRefreshTokenCommand { UserId = userId }, cancellationToken);
            return result.ToActionResult();
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
