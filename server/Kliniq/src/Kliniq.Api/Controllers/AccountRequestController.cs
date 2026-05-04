using Kliniq.Api.Models.Requests;
using Kliniq.Application.Common.Models;
using Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest;
using Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest;
using Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [Route("api/account-requests")]
    [ApiController]
    public class AccountRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("submit")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Submit([FromForm] SubmitAccountRequestRequest request, CancellationToken cancellationToken)
        {
            var command = new SubmitAccountRequestCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Specialization = request.Specialization,
                Street = request.Street,
                City = request.City,
                Country = request.Country,
                PrcId = ToFileUpload(request.PrcId),
                BoardCertificate = ToFileUpload(request.BoardCertificate),
                MedicalDiploma = ToFileUpload(request.MedicalDiploma),
                CertificateOfGoodStanding = ToFileUpload(request.CertificateOfGoodStanding)
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Approve(Guid id, [FromBody]ApproveAccountRequestRequest request, CancellationToken cancellationToken)
        {
            var command = new ApproveAccountRequestCommand
            {
                AccountRequestId = id,
                AdminNote = request.AdminNote
            };

            await _mediator.Send(command, cancellationToken);
            return Ok(new { message = "Accoutn request approved and invitation email sent" });
        }

        [HttpPost("{id}/reject")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Rejec(Guid id, [FromBody] RejectAccountRequestRequest request, CancellationToken cancellationToken)
        {
            var command = new RejectAccountRequestCommand
            {
                AccountRequestId = id,
                AdminNote = request.AdminNote
            };

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
