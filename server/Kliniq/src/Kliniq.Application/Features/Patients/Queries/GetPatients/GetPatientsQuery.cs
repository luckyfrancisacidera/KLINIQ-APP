using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Queries.GetPatients
{
    public sealed record GetPatientsQuery(
        string? Search = null,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<Result<PagedResult<PatientDto>>>;
}
