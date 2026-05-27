using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Queries.GetAvailableSlots
{
    public sealed class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, Result<IReadOnlyList<AvailableSlotDto>>>
    {
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        public GetAvailableSlotsQueryHandler(IPractitionerRepository practitionerRepository, IScheduleRepository scheduleRepository)
        {
            _practitionerRepository = practitionerRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Result<IReadOnlyList<AvailableSlotDto>>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            var practitionerExists = await _practitionerRepository.ExistsAsync(request.PractitionerId, cancellationToken);

            if (!practitionerExists)
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}'"));

            if (!string.IsNullOrWhiteSpace(request.Day) && !Enum.TryParse<ClinicDayOfWeek>(request.Day, ignoreCase: true, out _))
                return Result.Failure<IReadOnlyList<AvailableSlotDto>>(Error.Validation("Schedule.InvalidDay", $"'{request.Day}' is not a valid of the week."));

            var schedules = await _scheduleRepository.GetByPractitionerIdAsync(request.PractitionerId, cancellationToken);

            var result = schedules
                .Where(s => s.IsAvailable)
                .Where(s => string.IsNullOrWhiteSpace(request.Day) || s.Day.ToString().Equals(request.Day, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Day)
                .Select(s => new AvailableSlotDto(
                    s.Day.ToString(),
                    s.GetAvailableSlots().Select(slot => slot.ToString("HH:mm")).ToList()
                )).ToList();

            return Result.Success<IReadOnlyList<AvailableSlotDto>>(result);
        }

    }
}
