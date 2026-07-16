using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.UpdateSchedule
{
    public sealed class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result<ScheduleSummaryDto>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateScheduleCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<ScheduleSummaryDto>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdWithBreaksAsync(request.ScheduleId, cancellationToken);

            if(schedule is null)
                return Result.Failure<ScheduleSummaryDto>(Error.NotFound("Schedule.NotFound",$"Schedule '{request.ScheduleId}' was not found."));

            var newDay = Enum.Parse<ClinicDayOfWeek>(request.Day, ignoreCase: true);
            var newStart = TimeOnly.ParseExact(request.StartTime, "HH:mm");
            var newEnd = TimeOnly.ParseExact(request.EndTime, "HH:mm");

            bool overlaps = await _scheduleRepository.HasTimeOverlapAsync(schedule.PractitionerId, (int)newDay, newStart, newEnd, excludeId: request.ScheduleId, cancellationToken);

            if(overlaps)
                return Result.Failure<ScheduleSummaryDto>(Error.Conflict("Schedule.Overlap", $"The time range {request.StartTime} - {request.EndTime} on {request.Day} overlaps with an existing schedule"));

            schedule.UpdateTimeSlot(newDay, newStart, newEnd, request.AppointmentLengthMinutes);

            _scheduleRepository.Update(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(schedule.ToSummaryDto());
        }
    }
}
