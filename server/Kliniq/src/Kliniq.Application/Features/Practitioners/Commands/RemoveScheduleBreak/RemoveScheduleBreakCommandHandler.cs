using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.RemoveScheduleBreak
{
    public sealed class RemoveScheduleBreakCommandHandler : IRequestHandler<RemoveScheduleBreakCommand, Result<ScheduleSummaryDto>>
    {
        private readonly IScheduleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveScheduleBreakCommandHandler(IScheduleRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScheduleSummaryDto>> Handle(RemoveScheduleBreakCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdWithBreaksTrackedAsync(request.ScheduleId, cancellationToken);

            if(schedule is null)
                return Result.Failure<ScheduleSummaryDto>(Error.NotFound("Schedule.NotFound", $"Schedule with id {request.ScheduleId} was not found."));

            schedule.RemoveBreak(request.BreakId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(schedule.ToSummaryDto());
        }
    }
}
