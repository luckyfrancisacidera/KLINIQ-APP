using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Clinics.DTOs;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using MediatR;

namespace Kliniq.Application.Features.Clinics.Queries.SearchClinics
{
    public sealed class SearchClinicsQueryHandler : IRequestHandler<SearchClinicsQuery, Result<PagedResult<ClinicSummaryDto>>>
    {
        private readonly IClinicRepository _repository;

        public SearchClinicsQueryHandler(IClinicRepository repository) => _repository = repository;

        public async Task<Result<PagedResult<ClinicSummaryDto>>> Handle(SearchClinicsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.SearchAsync(
                request.Search,
                request.Specialization,
                request.Latitude,
                request.Longitude,
                request.RadiusKm,
                request.SortBy,
                request.Page,
                request.PageSize,
                cancellationToken);

            var items = result.Items.Select(c => Map(c, request.Latitude, request.Longitude)).ToList();
            return Result.Success(new PagedResult<ClinicSummaryDto>(items, result.TotalItems, result.Page, result.PageSize));
        }

        private static ClinicSummaryDto Map(Clinic clinic, double? latitude, double? longitude)
        {
            var specializations = clinic.Practitioners
                .SelectMany(p => p.Specializations)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value)
                .ToList();

            return new ClinicSummaryDto(
                clinic.Id,
                clinic.Name,
                clinic.Location.Latitude,
                clinic.Location.Longitude,
                latitude.HasValue && longitude.HasValue
                    ? CalculateDistanceKm(latitude.Value, longitude.Value, clinic.Location.Latitude, clinic.Location.Longitude)
                    : null,
                clinic.Practitioners.Count,
                specializations);
        }

        private static double CalculateDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double earthRadiusKm = 6371d;
            var latitudeDelta = DegreesToRadians(latitude2 - latitude1);
            var longitudeDelta = DegreesToRadians(longitude2 - longitude1);
            var firstLatitude = DegreesToRadians(latitude1);
            var secondLatitude = DegreesToRadians(latitude2);

            var value = Math.Sin(latitudeDelta / 2) * Math.Sin(latitudeDelta / 2) +
                        Math.Cos(firstLatitude) * Math.Cos(secondLatitude) *
                        Math.Sin(longitudeDelta / 2) * Math.Sin(longitudeDelta / 2);

            return Math.Round(earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(value), Math.Sqrt(1 - value)), 1);
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180d;
    }
}
