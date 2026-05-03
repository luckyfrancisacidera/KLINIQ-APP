using Kliniq.Api.Models.Requests;
using Kliniq.Application.Common.Models;
using Kliniq.Application.Features.AccountRequests.Commands;
using MediatR;
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
        public async Task<IActionResult> Submit([FromForm] SubmitAccountRequestRequest request)
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

            var result = await _mediator.Send(command);
            return Ok(result);
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
