using FluentValidation;
using Kliniq.Domain.Enums;
using System.Text.RegularExpressions;

namespace Kliniq.Application.Features.Practitioners.Commands.CreateSchedule
{
    public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
    {

        private static readonly string TimePattern = @"^\d{2}:\d{2}$";
        public CreateScheduleCommandValidator()
        {
            RuleFor(x => x.PractitionerId).NotEmpty().WithMessage("Practitioner ID is required.");

            RuleFor(x => x.Day)
                .NotEmpty().WithMessage("Day is required.")
                .Must(d => Enum.TryParse<ClinicDayOfWeek>(d, true, out _))
                .WithMessage("Day must be one of: Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.")
                .Matches(TimePattern).WithMessage("Start time must be in a valid format (e.g., HH:mm).");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .Matches(TimePattern)
                .WithMessage("End time must be in a valid format (e.g., HH:mm).");

            RuleFor(x => x.AppointmentLengthMinutes)
                .GreaterThanOrEqualTo(10).WithMessage("Appointment length must be at least 10 minutes.")
                .LessThanOrEqualTo(480).WithMessage("Appointment length must be not exceed 8 hours.");  
            
            RuleFor(x => x)
                .Must(x => 
                    TimeOnly.TryParseExact(x.StartTime, "HH:mm", out var start) &&
                    TimeOnly.TryParseExact(x.EndTime, "HH:mm", out var end) &&
                    (end - start).TotalMinutes >= x.AppointmentLengthMinutes)
                .WithName("Appointment Fit")
                .WithMessage("Schedule must be at least one appointment length long and end time must be after start time.")
                .When(x => Regex.IsMatch(x.StartTime, TimePattern) && Regex.IsMatch(x.EndTime, TimePattern));
        }
    }
}
