using Kliniq.Api.Extensions;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.Commands.BookAppointment;
using Kliniq.Application.Features.Appointments.Commands.CancelAppointment;
using Kliniq.Application.Features.Appointments.Commands.CompleteAppointment;
using Kliniq.Application.Features.Appointments.Commands.ConfirmAppointment;
using Kliniq.Application.Features.Appointments.Commands.RescheduleAppointment;
using Kliniq.Application.Features.Appointments.Commands.QueueAppointment;
using Kliniq.Application.Features.Appointments.Commands.StartConsultation;
using Kliniq.Application.Features.Appointments.Queries.GetAppointment;
using Kliniq.Application.Features.Appointments.Queries.GetPatientAppointments;
using Kliniq.Application.Features.Appointments.Queries.GetPractitionerAppointments;
using Kliniq.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kliniq.Api.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public sealed class AppointmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPractitionerRepository _practitionerRepository;

        public AppointmentController(
            IMediator mediator,
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IPractitionerRepository practitionerRepository)
        {
            _mediator = mediator;
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _practitionerRepository = practitionerRepository;
        }

        [HttpGet("{id:guid}", Name = nameof(GetAppointment))]
        [Authorize(Roles = "Patient,Practitioner,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointment(Guid id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();
            if (!await CanAccessAsync(appointment, cancellationToken)) return Forbid();

            var result = await _mediator.Send(new GetAppointmentQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("patient/{patientId:guid}")]
        [Authorize(Roles = "Patient,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPatient(
            Guid patientId,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (!User.IsInRole("Admin") && !await OwnsPatientAsync(patientId, cancellationToken))
                return Forbid();

            var result = await _mediator.Send(new GetPatientAppointmentsQuery(patientId, status, dateFrom, dateTo, page, pageSize), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("practitioner/{practitionerId:guid}")]
        [Authorize(Roles = "Practitioner,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPractitioner(
            Guid practitionerId,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (!User.IsInRole("Admin") && !await OwnsPractitionerAsync(practitionerId, cancellationToken))
                return Forbid();

            var result = await _mediator.Send(new GetPractitionerAppointmentsQuery(practitionerId, status, dateFrom, dateTo, page, pageSize), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Book([FromBody] BookAppointmentCommand command, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var result = await _mediator.Send(command with { UserId = userId }, cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return CreatedAtAction(nameof(GetAppointment), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPost("{id:guid}/reschedule")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentRequest request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();
            if (!await OwnsPatientAsync(appointment.PatientId, cancellationToken)) return Forbid();

            var result = await _mediator.Send(new RescheduleAppointmentCommand(
                id,
                request.ScheduleId,
                request.AppointmentDate,
                request.SlotTime), cancellationToken);
            return result.ToActionResult();
        }

        public sealed record RescheduleAppointmentRequest(Guid ScheduleId, DateOnly AppointmentDate, TimeOnly SlotTime);

        [HttpPost("{id:guid}/confirm")]
        [Authorize(Roles = "Practitioner")]
        public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();
            if (!await OwnsPractitionerAsync(appointment.PractitionerId, cancellationToken)) return Forbid();
            var result = await _mediator.Send(new ConfirmAppointmentCommand(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/queue")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Queue(Guid id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();
            if (!await OwnsPractitionerAsync(appointment.PractitionerId, cancellationToken)) return Forbid();
            var result = await _mediator.Send(new QueueAppointmentCommand(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/start-consultation")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> StartConsultation(Guid id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();
            if (!await OwnsPractitionerAsync(appointment.PractitionerId, cancellationToken)) return Forbid();
            var result = await _mediator.Send(new StartConsultationCommand(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/cancel")]
        [Authorize(Roles = "Patient,Practitioner")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();
            if (!await CanAccessAsync(appointment, cancellationToken)) return Forbid();
            var result = await _mediator.Send(new CancelAppointmentCommand(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/complete")]
        [Authorize(Roles = "Practitioner")]
        public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteAppointmentRequest? request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();
            if (!await OwnsPractitionerAsync(appointment.PractitionerId, cancellationToken)) return Forbid();
            var result = await _mediator.Send(new CompleteAppointmentCommand(id, request?.Notes), cancellationToken);
            return result.ToActionResult();
        }

        public sealed record CompleteAppointmentRequest(string? Notes);

        private bool TryGetUserId(out Guid userId)
            => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

        private async Task<bool> OwnsPatientAsync(Guid patientId, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return false;
            var patient = await _patientRepository.GetByUserIdAsync(userId, cancellationToken);
            return patient?.Id == patientId;
        }

        private async Task<bool> OwnsPractitionerAsync(Guid practitionerId, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return false;
            var practitioner = await _practitionerRepository.GetByUserIdAsync(userId, cancellationToken);
            return practitioner?.Id == practitionerId;
        }

        private async Task<bool> CanAccessAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            if (User.IsInRole("Admin")) return true;
            if (User.IsInRole("Patient")) return await OwnsPatientAsync(appointment.PatientId, cancellationToken);
            if (User.IsInRole("Practitioner")) return await OwnsPractitionerAsync(appointment.PractitionerId, cancellationToken);
            return false;
        }
    }
}
