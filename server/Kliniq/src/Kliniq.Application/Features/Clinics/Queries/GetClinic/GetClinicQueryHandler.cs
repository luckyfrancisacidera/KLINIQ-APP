using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Clinics.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Clinics.Queries.GetClinic
{
    public sealed class GetClinicQueryHandler : IRequestHandler<GetClinicQuery, Result<ClinicDetailDto>>
    {
        private readonly IClinicRepository _repository;

        public GetClinicQueryHandler(IClinicRepository repository) => _repository = repository;

        public async Task<Result<ClinicDetailDto>> Handle(GetClinicQuery request, CancellationToken cancellationToken)
        {
            var clinic = await _repository.GetByIdAsync(request.ClinicId, cancellationToken);
            if (clinic is null)
                return Result.Failure<ClinicDetailDto>(Error.NotFound("Clinic.NotFound", "Clinic was not found."));

            var practitioners = clinic.Practitioners
                .OrderBy(p => p.Name.LastName)
                .ThenBy(p => p.Name.FirstName)
                .Select(p => new ClinicPractitionerSummaryDto(
                    p.Id,
                    p.Name.FirstName,
                    p.Name.LastName,
                    p.Specializations))
                .ToList();

            return Result.Success(new ClinicDetailDto(
                clinic.Id,
                clinic.Name,
                clinic.Location.Latitude,
                clinic.Location.Longitude,
                practitioners));
        }
    }
}
