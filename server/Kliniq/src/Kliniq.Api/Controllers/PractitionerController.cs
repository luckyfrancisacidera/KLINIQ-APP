using Kliniq.Api.Extensions;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.Commands.AddScheduleBreak;
using Kliniq.Application.Features.Practitioners.Commands.CreateSchedule;
using Kliniq.Application.Features.Practitioners.Commands.DeletePractitioner;
using Kliniq.Application.Features.Practitioners.Commands.DeleteSchedule;
using Kliniq.Application.Features.Practitioners.Commands.RemoveScheduleBreak;
using Kliniq.Application.Features.Practitioners.Commands.UpdatePractitioner;
using Kliniq.Application.Features.Practitioners.Commands.UpdateSchedule;
using Kliniq.Application.Features.Practitioners.Queries.GetAvailableSlots;
using Kliniq.Application.Features.Practitioners.Queries.GetCurrentPractitioner;
using Kliniq.Application.Features.Practitioners.Queries.GetPractitioner;
using Kliniq.Application.Features.Practitioners.Queries.GetPractitioners;
using Kliniq.Application.Features.Practitioners.Queries.GetSchedules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kliniq.Api.Controllers
{
    [Route("api/practitioners")]
    [ApiController]
    [Produces("application/json")]
    public sealed class PractitionerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public PractitionerController(
            IMediator mediator,
            IPractitionerRepository practitionerRepository,
            IScheduleRepository scheduleRepository)
        {
            _mediator = mediator;
            _practitionerRepository = practitionerRepository;
            _scheduleRepository = scheduleRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? specialization,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPractitionersQuery(search, specialization, page, pageSize), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("me")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var result = await _mediator.Send(new GetCurrentPractitionerQuery(userId), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}", Name = nameof(GetPractitioner))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPractitioner(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPractitionerQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePractitionerCommand command, CancellationToken cancellationToken)
        {
            if (!await OwnsPractitionerAsync(id, cancellationToken)) return Forbid();
            var result = await _mediator.Send(command with { PractitionerId = id }, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeletePractitionerCommand(id), cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return NoContent();
        }

        [HttpGet("{id:guid}/schedules")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSchedules(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSchedulesQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}/available-slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetAvailableSlots(
            Guid id,
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAvailableSlotsQuery(id, from, to), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/schedules")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateSchedule(Guid id, [FromBody] CreateScheduleCommand command, CancellationToken cancellationToken)
        {
            if (!await OwnsPractitionerAsync(id, cancellationToken)) return Forbid();
            var result = await _mediator.Send(command with { PractitionerId = id }, cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return CreatedAtAction(nameof(GetSchedules), new { id }, result.Value);
        }

        [HttpPut("{id:guid}/schedules/{scheduleId:guid}")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateSchedule(Guid id, Guid scheduleId, [FromBody] UpdateScheduleCommand command, CancellationToken cancellationToken)
        {
            if (!await OwnsPractitionerAsync(id, cancellationToken)) return Forbid();
            if (!await ScheduleBelongsToPractitionerAsync(scheduleId, id, cancellationToken)) return Forbid();
            var result = await _mediator.Send(command with { ScheduleId = scheduleId }, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}/schedules/{scheduleId:guid}")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteSchedule(Guid id, Guid scheduleId, CancellationToken cancellationToken)
        {
            if (!await OwnsPractitionerAsync(id, cancellationToken)) return Forbid();
            if (!await ScheduleBelongsToPractitionerAsync(scheduleId, id, cancellationToken)) return Forbid();
            var result = await _mediator.Send(new DeleteScheduleCommand(scheduleId), cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return NoContent();
        }

        [HttpPost("{id:guid}/schedules/{scheduleId:guid}/breaks")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddBreak(Guid id, Guid scheduleId, [FromBody] AddScheduleBreakCommand command, CancellationToken cancellationToken)
        {
            if (!await OwnsPractitionerAsync(id, cancellationToken)) return Forbid();
            if (!await ScheduleBelongsToPractitionerAsync(scheduleId, id, cancellationToken)) return Forbid();
            var result = await _mediator.Send(command with { ScheduleId = scheduleId }, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}/schedules/{scheduleId:guid}/breaks/{breakId:guid}")]
        [Authorize(Roles = "Practitioner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteBreak(Guid id, Guid scheduleId, Guid breakId, CancellationToken cancellationToken)
        {
            if (!await OwnsPractitionerAsync(id, cancellationToken)) return Forbid();
            if (!await ScheduleBelongsToPractitionerAsync(scheduleId, id, cancellationToken)) return Forbid();
            var result = await _mediator.Send(new RemoveScheduleBreakCommand(scheduleId, breakId), cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return NoContent();
        }

        private bool TryGetUserId(out Guid userId)
            => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

        private async Task<bool> OwnsPractitionerAsync(Guid practitionerId, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return false;
            var practitioner = await _practitionerRepository.GetByUserIdAsync(userId, cancellationToken);
            return practitioner?.Id == practitionerId;
        }

        private async Task<bool> ScheduleBelongsToPractitionerAsync(
            Guid scheduleId,
            Guid practitionerId,
            CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
            return schedule?.PractitionerId == practitionerId;
        }
    }
}
