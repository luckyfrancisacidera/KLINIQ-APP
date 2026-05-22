using FluentValidation;
using Kliniq.Domain.Enums;

namespace Kliniq.Application.Features.Practitioners.Commands.CreateSchedule
{
    public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
    {
        public CreateScheduleCommandValidator()
        {
            RuleFor(x => x.PractitionerId).NotEmpty().WithMessage("Practitioner ID is required.");

            RuleFor(x => x.Day)
                .NotEmpty().WithMessage("Day is required.")
                .Must(d => Enum.TryParse<ClinicDayOfWeek>(d, true, out _))
                .WithMessage("Day must be a valid day of the week.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.")
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("Start time must be in a valid format (e.g., HH:mm).");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("End time must be in a valid format (e.g., HH:mm).");

            RuleFor(x => x.AppointmentLengthMinutes)
                .GreaterThanOrEqualTo(10).WithMessage("Appointment length must be at least 10 minutes.")
                .LessThanOrEqualTo(480).WithMessage("Appointment length must be not exceed 8 minutes.");       
        }
    }
}
