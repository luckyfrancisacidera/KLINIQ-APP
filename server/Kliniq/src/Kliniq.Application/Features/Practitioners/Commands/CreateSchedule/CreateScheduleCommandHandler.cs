using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Application.Features.Practitioners.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.CreateSchedule
{
    public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Result<ScheduleSummaryDto>>
    {
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduleCommandHandler(IPractitionerRepository practitionerRepository, IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
        {
            _practitionerRepository = practitionerRepository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScheduleSummaryDto>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        {
            var practitoner = await _practitionerRepository.GetByIdAsync(request.PractitionerId, cancellationToken);

            if (practitoner is null)
                return Result.Failure<ScheduleSummaryDto>(Error.NotFound("Practitioner.NotFound", $"Practitioner '{request.PractitionerId}' was not found"));
            
            var day = Enum.Parse<ClinicDayOfWeek>(request.Day, true);
            var startTime = TimeOnly.ParseExact(request.StartTime, "HH:mm");
            var endTime = TimeOnly.ParseExact(request.EndTime, "HH:mm");

            bool hasOverlap = await _scheduleRepository.HasOverlappingScheduleAsync(request.PractitionerId, (int)day, null, cancellationToken);

            if(hasOverlap)
                return Result.Failure<ScheduleSummaryDto>(Error.Conflict("Schedule.Overlap", $"The practitioner already has a schedule for {request.Day}"));

            var schedule = new Schedule(
                request.PractitionerId,
                day,
                startTime,
                endTime,
                request.AppointmentLengthMinutes
                );

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(schedule.ToSummaryDto());
        }
    }

}
