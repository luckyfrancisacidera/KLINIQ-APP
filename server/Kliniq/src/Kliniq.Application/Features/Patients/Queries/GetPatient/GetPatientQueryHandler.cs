using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Application.Features.Patients.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Queries.GetPatient
{
    public sealed class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, Result<PatientDto>>
    {
        private readonly IPatientRepository _patientRepository;

        public GetPatientQueryHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<Result<PatientDto>> Handle(GetPatientQuery request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

            if (patient == null)
                return Result.Failure<PatientDto>(Error.NotFound("Patient.NotFound", $"Patient '{request.PatientId}' not found"));

            return Result.Success(patient.ToDto());
        }

    }
}
