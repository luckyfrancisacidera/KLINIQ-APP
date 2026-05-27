using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Application.Features.Patients.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.Patients.Commands.UpdatePatient
{
    public sealed class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result<PatientDto>>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
        {
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdTrackedAsync(request.PatientId, cancellationToken);

            if(patient is null)
                return Result.Failure<PatientDto>(Error.NotFound("Patient.NotFound", $"Patient '{request.PatientId}' not found"));

            var name = new FullName(request.FirstName, request.LastName);
            var address = new Address(request.Street, request.City, request.Country);

            patient.UpdateProfile(name, address, request.PhoneNumber, request.EmergencyContact);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(patient.ToDto());
        }
    }
}
