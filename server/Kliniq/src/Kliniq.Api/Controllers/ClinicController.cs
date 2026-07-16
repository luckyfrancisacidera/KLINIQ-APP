using Kliniq.Api.Extensions;
using Kliniq.Application.Features.Clinics.Queries.GetClinic;
using Kliniq.Application.Features.Clinics.Queries.SearchClinics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Controllers
{
    [ApiController]
    [Route("api/clinics")]
    [Produces("application/json")]
    public sealed class ClinicController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClinicController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] string? specialization,
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double? radiusKm,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new SearchClinicsQuery(
                search,
                specialization,
                latitude,
                longitude,
                radiusKm,
                sortBy,
                page,
                pageSize), cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetClinicQuery(id), cancellationToken);
            return result.ToActionResult();
        }
    }
}
