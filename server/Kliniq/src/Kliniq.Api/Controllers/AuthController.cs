using Kliniq.Api.Models.Requests;
using Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword;
using Kliniq.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request, CancellationToken cancellationToken)
        {
            var command = new SetPractitionerPasswordCommand
            {
                InvitationToken = request.InvitationToken,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                ClinicId = request.ClinicId,
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
