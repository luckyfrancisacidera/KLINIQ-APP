using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetSchedules
{
    public sealed class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQuery, Result<IReadOnlyList<ScheduleSummaryDto>>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IPractitionerRepository _practitionerRepository;
        public GetSchedulesQueryHandler(IScheduleRepository scheduleRepository, IPractitionerRepository practitionerRepository)
        {
            _scheduleRepository = scheduleRepository;
            _practitionerRepository = practitionerRepository;
        }

        public async Task<Result<IReadOnlyList<ScheduleSummaryDto>>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
        {
            var practitionerExists = await _practitionerRepository.ExistsAsync(request.PractitionerId, cancellationToken);

            if (!practitionerExists)
                return Result.Failure<IReadOnlyList<ScheduleSummaryDto>>(Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}' not found."));

            var schedules = await _scheduleRepository.GetByPractitionerIdAsync(request.PractitionerId, cancellationToken);

            var dtos = schedules.Select(s => s.ToSummaryDto()).ToList();
            return Result.Success<IReadOnlyList<ScheduleSummaryDto>>(dtos);
        }
    }
}
