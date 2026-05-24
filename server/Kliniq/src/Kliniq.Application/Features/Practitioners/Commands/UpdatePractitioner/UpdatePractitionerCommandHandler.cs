using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.UpdatePractitioner
{
    public sealed class UpdatePractitionerCommandHandler : IRequestHandler<UpdatePractitionerCommand, Result<PractitionerDto>>
    {
        private readonly IPractitionerRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePractitionerCommandHandler(IPractitionerRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<PractitionerDto>> Handle(UpdatePractitionerCommand request, CancellationToken cancellationToken)
        {
            var practitioner = await _repository.GetByIdTrackedAsync(request.PractitionerId, cancellationToken);

            if(practitioner is null)
                return Result.Failure<PractitionerDto>(Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}' was not found."));

            var newName = new FullName(request.FirstName, request.LastName);

            practitioner.UpdateProfile(newName, request.Specializations.AsReadOnly());

            _repository.Update(practitioner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(practitioner.ToDto());
        }
    }
}
