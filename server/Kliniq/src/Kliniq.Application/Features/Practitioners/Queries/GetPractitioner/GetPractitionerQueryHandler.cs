using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetPractitioner
{
    public sealed class GetPractitionerQueryHandler : IRequestHandler<GetPractitionerQuery, Result<PractitionerDetailDto>>
    {
        private readonly IPractitionerRepository _repository;

        public GetPractitionerQueryHandler(IPractitionerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PractitionerDetailDto>> Handle(GetPractitionerQuery request, CancellationToken cancellationToken)
        {
            var practitioner = await _repository.GetByIdWithSchedulesAsync(request.PractitionerId, cancellationToken);

            if(practitioner is null)
                return Result.Failure<PractitionerDetailDto>(Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}' not found."));

            return Result.Success(practitioner.ToDetailDto());
        }
    }
}
