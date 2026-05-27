using Kliniq.Api.Extensions;
using Kliniq.Application.Features.Practitioners.Commands.AddScheduleBreak;
using Kliniq.Application.Features.Practitioners.Commands.CreateSchedule;
using Kliniq.Application.Features.Practitioners.Commands.DeletePractitioner;
using Kliniq.Application.Features.Practitioners.Commands.RemoveScheduleBreak;
using Kliniq.Application.Features.Practitioners.Commands.UpdatePractitioner;
using Kliniq.Application.Features.Practitioners.Commands.UpdateSchedule;
using Kliniq.Application.Features.Practitioners.Queries.GetAvailableSlots;
using Kliniq.Application.Features.Practitioners.Queries.GetPractitioner;
using Kliniq.Application.Features.Practitioners.Queries.GetPractitioners;
using Kliniq.Application.Features.Practitioners.Queries.GetSchedules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [Route("api/practitioners")]
    [ApiController]
    public class PractitionerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PractitionerController(IMediator mediator) => _mediator = mediator;

        // Practitioner CRUD endpoints
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? specialization,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send( new GetPractitionersQuery(search, specialization, page, pageSize), cancellationToken);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePractitionerCommand command, CancellationToken cancellationToken)
        {
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

        //Schedule Management Endpoints
        [HttpGet("{id:guid}/schedules")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSchedules(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSchedulesQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}/available-slots")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetAvailableSlots(Guid id, [FromQuery] string? day, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAvailableSlotsQuery(id, day), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/schedules")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateSchedule(Guid id, [FromBody] CreateScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { PractitionerId = id }, cancellationToken);
            if (result.IsFailure) return result.ToActionResult();
            return CreatedAtAction(nameof(GetSchedules), new { id }, null);

        }

        [HttpPut("{id:guid}/schedules/{scheduleId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateSchedule(Guid id, Guid scheduleId, [FromBody] UpdateScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { ScheduleId = scheduleId }, cancellationToken);
            return result.ToActionResult();

        }

        //Break Management Endpoints
        [HttpPost("{id:guid}/schedules/{scheduleId:guid}/breaks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddBreak(Guid id, Guid scheduleId, [FromBody] AddScheduleBreakCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { ScheduleId = scheduleId }, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}/schedules/{scheduleId:guid}/breaks/{breakId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBreak(Guid id, Guid scheduleId, Guid breakId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveScheduleBreakCommand(scheduleId, breakId), cancellationToken);
           
            if(result.IsFailure) return result.ToActionResult();
            return NoContent();
        }

    }

}
