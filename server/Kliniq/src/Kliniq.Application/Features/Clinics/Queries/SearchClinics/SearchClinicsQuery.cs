using Kliniq.Application.Features.Clinics.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Clinics.Queries.SearchClinics
{
    public sealed record SearchClinicsQuery(
        string? Search,
        string? Specialization,
        double? Latitude,
        double? Longitude,
        double? RadiusKm,
        string? SortBy,
        int Page = 1,
        int PageSize = 20) : IRequest<Result<PagedResult<ClinicSummaryDto>>>;
}
