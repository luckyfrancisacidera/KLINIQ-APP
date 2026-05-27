using Kliniq.Api.Extensions;
using Kliniq.Api.Models;
using Kliniq.Application.Common.Models;
using Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest;
using Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest;
using Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [Route("api/account-requests")]
    [ApiController]
    public class AccountRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountRequestController(IMediator mediator) => _mediator = mediator;
     
        [HttpPost("submit")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]    
        public async Task<IActionResult> Submit(
            [FromForm] SubmitAccountRequestForm form,
            CancellationToken cancellationToken)
        {
            var command = new SubmitAccountRequestCommand
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                LicenseNumber = form.LicenseNumber,
                Specializations = form.Specializations,

                Street = form.Street,
                City = form.City,
                Country = form.Country,

                ClinicName = form.ClinicName,
                ClinicLatitude = form.ClinicLatitude,
                ClinicLongitude = form.ClinicLongitude,

                PrcLicense = ToFileUpload(form.PrcLicense!),
                GovernmentId = ToFileUpload(form.GovernmentId!),
                ProfessionalPhoto = ToFileUpload(form.ProfessionalPhoto!),
                Cv = ToFileUpload(form.Cv!),
            };

            var result = await _mediator.Send(command, cancellationToken);

            if(result.IsFailure) return result.ToActionResult();

            return CreatedAtAction(null, result.Value);   
        }

        [HttpPost("{id}/approve")]
        //[Authorize(Roles ="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveAccountRequestCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { message = "Account request approved and invitation email sent" });
        }

        [HttpPost("{id}/reject")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Rejec(Guid id, [FromBody] RejectAccountRequestCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command,cancellationToken);
            return Ok(new { message = "Account request rejected and notification email sent" });
        }

        private static FileUpload ToFileUpload(IFormFile file) => new()
        {
            Content = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length
        };
    }

}
