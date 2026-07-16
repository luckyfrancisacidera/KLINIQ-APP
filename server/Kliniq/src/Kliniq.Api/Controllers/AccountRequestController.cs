using Kliniq.Api.Extensions;
using Kliniq.Api.Models;
using Kliniq.Application.Common.Models;
using Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest;
using Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest;
using Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest;
using Kliniq.Application.Features.AccountRequests.Queries.GetAccountRequest;
using Kliniq.Application.Features.AccountRequests.Queries.GetAccountRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers;

[Route("api/account-requests")]
[Authorize]
[ApiController]
[Produces("application/json")]
public sealed class AccountRequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountRequestController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAccountRequestsQuery(page, pageSize, search, status), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountRequestQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("submit")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Submit([FromForm] SubmitAccountRequestForm form, CancellationToken cancellationToken)
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
        if (result.IsFailure) return result.ToActionResult();
        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveAccountRequestBody body, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveAccountRequestCommand
        {
            AccountRequestId = id,
            AdminNote = body.Notes,
        }, cancellationToken);

        if (result.IsFailure) return result.ToActionResult();
        return Ok(new { message = "Account request approved and invitation email sent." });
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectAccountRequestBody body, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectAccountRequestCommand
        {
            AccountRequestId = id,
            AdminNote = body.Reason,
        }, cancellationToken);

        if (result.IsFailure) return result.ToActionResult();
        return Ok(new { message = "Account request rejected and notification email sent." });
    }

    public sealed record ApproveAccountRequestBody(string? Notes);
    public sealed record RejectAccountRequestBody(string Reason);

    private static FileUpload ToFileUpload(IFormFile file) => new()
    {
        Content = file.OpenReadStream(),
        FileName = file.FileName,
        ContentType = file.ContentType,
        Size = file.Length,
    };
}
