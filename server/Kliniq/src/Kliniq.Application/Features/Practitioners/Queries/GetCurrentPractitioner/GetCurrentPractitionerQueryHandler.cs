using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetCurrentPractitioner
{
    public sealed class GetCurrentPractitionerQueryHandler : IRequestHandler<GetCurrentPractitionerQuery, Result<PractitionerDetailDto>>
    {
        private readonly IPractitionerRepository _repository;
        public GetCurrentPractitionerQueryHandler(IPractitionerRepository repository) => _repository = repository;

        public async Task<Result<PractitionerDetailDto>> Handle(GetCurrentPractitionerQuery request, CancellationToken cancellationToken)
        {
            var practitioner = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (practitioner is null)
                return Result.Failure<PractitionerDetailDto>(Error.NotFound("Practitioner.NotFound", "Practitioner profile was not found."));

            var detailed = await _repository.GetByIdWithSchedulesAsync(practitioner.Id, cancellationToken);
            return detailed is null
                ? Result.Failure<PractitionerDetailDto>(Error.NotFound("Practitioner.NotFound", "Practitioner profile was not found."))
                : Result.Success(detailed.ToDetailDto());
        }
    }
}
