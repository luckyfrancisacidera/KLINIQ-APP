using Kliniq.Api.Extensions;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Patients.Commands.DeletePatient;
using Kliniq.Application.Features.Patients.Commands.UpdatePatient;
using Kliniq.Application.Features.Patients.Queries.GetCurrentPatient;
using Kliniq.Application.Features.Patients.Queries.GetPatient;
using Kliniq.Application.Features.Patients.Queries.GetPatients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kliniq.Api.Controllers
{
    [Route("api/patient")]
    [ApiController]
    [Produces("application/json")]
    public sealed class PatientController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPatientRepository _patientRepository;

        public PatientController(IMediator mediator, IPatientRepository patientRepository)
        {
            _mediator = mediator;
            _patientRepository = patientRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPatientsQuery(search, page, pageSize), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("me")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var result = await _mediator.Send(new GetCurrentPatientQuery(userId), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}", Name = nameof(GetPatient))]
        [Authorize(Roles = "Admin,Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPatient(Guid id, CancellationToken cancellationToken)
        {
            if (User.IsInRole("Patient") && !await OwnsPatientAsync(id, cancellationToken))
                return Forbid();

            var result = await _mediator.Send(new GetPatientQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientCommand command, CancellationToken cancellationToken)
        {
            if (!await OwnsPatientAsync(id, cancellationToken)) return Forbid();
            var result = await _mediator.Send(command with { PatientId = id }, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeletePatientCommand(id), cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return NoContent();
        }

        private bool TryGetUserId(out Guid userId)
            => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

        private async Task<bool> OwnsPatientAsync(Guid patientId, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return false;
            var patient = await _patientRepository.GetByUserIdAsync(userId, cancellationToken);
            return patient?.Id == patientId;
        }
    }
}
