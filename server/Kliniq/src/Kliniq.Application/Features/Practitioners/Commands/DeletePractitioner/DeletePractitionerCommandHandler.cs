using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.DeletePractitioner
{
    public sealed record DeletePractitionerCommand(Guid PractitionerId) : IRequest<Result>;

    public sealed class DeletePractitionerCommandHandler : IRequestHandler<DeletePractitionerCommand, Result>
    {
        private readonly IPractitionerRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePractitionerCommandHandler(IPractitionerRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeletePractitionerCommand request, CancellationToken cancellationToken)
        {
            var practitioner = await _repository.GetByIdTrackedAsync(request.PractitionerId, cancellationToken);

            if (practitioner is null)
                return Result.Failure(Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}' was not found."));

            _repository.Delete(practitioner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
