using Kliniq.Api.Extensions;
using Kliniq.Application.Features.Patients.Commands.DeletePatient;
using Kliniq.Application.Features.Patients.Commands.UpdatePatient;
using Kliniq.Application.Features.Patients.Queries.GetPatient;
using Kliniq.Application.Features.Patients.Queries.GetPatients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [Route("api/patient")]
    [ApiController]
    //[Authorize]
    [Produces("application/json")]
    public class PatientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // PATIENT QUERIES ENDPOINTS

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPatientsQuery(page, pageSize), cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{id:guid}", Name = nameof(GetPatient))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPatient(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPatientQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(Guid id, [FromQuery] UpdatePatientCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { PatientId = id }, cancellationToken);
            return result.ToActionResult();

        }

        [HttpDelete("{id:guid}")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delte(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeletePatientCommand(id), cancellationToken);

            if(result.IsFailure) return result.ToActionResult();

            return NoContent();
        }
    }
}
