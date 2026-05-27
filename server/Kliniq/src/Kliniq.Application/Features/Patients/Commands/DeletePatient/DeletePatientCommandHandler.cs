using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Commands.DeletePatient
{
    public sealed record DeletePatientCommand(Guid PatientId) : IRequest<Result>;

    public sealed class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Result>
    {
        private readonly IPatientRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePatientCommandHandler(IPatientRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _repository.GetByIdTrackedAsync(request.PatientId, cancellationToken);

            if (patient is null)
                return Result.Failure(Error.NotFound("Patient.NotFound", $"Patient with id {request.PatientId} was not found."));
            
            _repository.Delete(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

    }
}
