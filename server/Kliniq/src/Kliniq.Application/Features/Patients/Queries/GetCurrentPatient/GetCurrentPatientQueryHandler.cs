using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Application.Features.Patients.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Queries.GetCurrentPatient
{
    public sealed class GetCurrentPatientQueryHandler : IRequestHandler<GetCurrentPatientQuery, Result<PatientDto>>
    {
        private readonly IPatientRepository _repository;
        public GetCurrentPatientQueryHandler(IPatientRepository repository) => _repository = repository;

        public async Task<Result<PatientDto>> Handle(GetCurrentPatientQuery request, CancellationToken cancellationToken)
        {
            var patient = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
            return patient is null
                ? Result.Failure<PatientDto>(Error.NotFound("Patient.NotFound", "Patient profile was not found."))
                : Result.Success(patient.ToDto());
        }
    }
}
