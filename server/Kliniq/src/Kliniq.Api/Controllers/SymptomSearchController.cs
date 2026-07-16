using Kliniq.Api.Extensions;
using Kliniq.Application.Features.SymptomSearch.Queries.SearchBySymptoms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Kliniq.Api.Controllers
{
    [ApiController]
    [Route("api/symptom-search")]
    [AllowAnonymous]
    [EnableRateLimiting("symptom-search")]
    [Produces("application/json")]
    public sealed class SymptomSearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SymptomSearchController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Search([FromBody] SymptomSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new SearchBySymptomsQuery(request.Symptoms, request.Page, request.PageSize),
                cancellationToken);

            return result.ToActionResult();
        }

        public sealed record SymptomSearchRequest(string Symptoms, int Page = 1, int PageSize = 6);
    }
}
