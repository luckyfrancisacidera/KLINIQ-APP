using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.AddScheduleBreak
{
    public sealed class AddScheduleBreakCommandHandler : IRequestHandler<AddSchedulelBreakCommand, Result<ScheduleSummaryDto>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddScheduleBreakCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScheduleSummaryDto>> Handle(AddSchedulelBreakCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdWithBreaksAsync(request.ScheduleId, cancellationToken);

            if (schedule is null)
                return Result.Failure<ScheduleSummaryDto>(Error.NotFound("Schedule.NotFound", $"Schedule '{request.ScheduleId}' was not found"));

            var breakStart = TimeOnly.ParseExact(request.BreakStart, "HH:mm");
            var breakEnd = TimeOnly.ParseExact(request.BreakEnd, "HH:mm");

            schedule.AddBreak(breakStart, breakEnd);

            _scheduleRepository.Update(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(schedule.ToSummaryDto());
        }
    }
}
