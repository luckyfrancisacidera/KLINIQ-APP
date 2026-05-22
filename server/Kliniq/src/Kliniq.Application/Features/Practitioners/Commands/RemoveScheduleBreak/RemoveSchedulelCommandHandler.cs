using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.RemoveScheduleBreak
{
    public sealed class RemoveSchedulelCommandHandler : IRequestHandler<RemoveScheduleCommand, Result<ScheduleSummaryDto>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveSchedulelCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScheduleSummaryDto>> Handle(RemoveScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);

            if(schedule is null)
                return Result.Failure<ScheduleSummaryDto>(Error.NotFound("Schedule.NotFound", $"Schedule with id {request.ScheduleId} was not found."));

            schedule.RemoveBreak(request.BreakId);

            _scheduleRepository.Update(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(schedule.ToSummaryDto());
        }
    }
}
