using Kliniq.Api.Extensions;
using Kliniq.Application.Features.Appointments.Commands.BookAppointment;
using Kliniq.Application.Features.Appointments.Commands.CancelAppointment;
using Kliniq.Application.Features.Appointments.Commands.CompleteAppointment;
using Kliniq.Application.Features.Appointments.Commands.ConfirmAppointment;
using Kliniq.Application.Features.Appointments.Queries.GetAppointment;
using Kliniq.Application.Features.Appointments.Queries.GetPatientAppointments;
using Kliniq.Application.Features.Appointments.Queries.GetPractitionerAppointments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    //[Authorize]
    [Produces("application/json")]
    public class AppointmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AppointmentController(IMediator mediator) => _mediator = mediator;

        // APPOINTMENT QUERIES ENDPOINTS

        [HttpGet("{id:guid}", Name = nameof(GetAppointment))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointment(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAppointmentQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("patient/{patientId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPatient(Guid patientId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPatientAppointmentsQuery(patientId, page, pageSize), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("practitioner/{practitionerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPractitioner(Guid practitionerId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPractitionerAppointmentsQuery(practitionerId, page, pageSize), cancellationToken);
            return result.ToActionResult();
        }

        // APPOINTMENT COMMANDS ENDPOINTS
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Book([FromBody] BookAppointmentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return CreatedAtAction(nameof(GetAppointment), new { id = result.Value!.Id }, result.Value);
        }
        [HttpPost("{id:guid}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ConfirmAppointmentCommand(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelAppointmentCommand(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CompleteAppointmentCommand(id), cancellationToken);
            return result.ToActionResult();
        }

    }
}

