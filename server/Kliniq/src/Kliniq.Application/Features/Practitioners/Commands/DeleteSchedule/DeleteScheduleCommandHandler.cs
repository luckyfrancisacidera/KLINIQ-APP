using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.DeleteSchedule
{
    public sealed record DeleteScheduleCommand(Guid ScheduleId) : IRequest<Result>;

    public sealed class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, Result>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteScheduleCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdTrackedAsync(request.ScheduleId, cancellationToken);

            if(schedule is null)
                return Result.Failure(Error.NotFound("Schedule.NotFound", $"Schedule '{request.ScheduleId}' was not found"));

            _scheduleRepository.Delete(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
