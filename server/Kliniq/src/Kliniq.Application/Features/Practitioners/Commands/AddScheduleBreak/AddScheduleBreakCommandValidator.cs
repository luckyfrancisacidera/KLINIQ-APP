using FluentValidation;

namespace Kliniq.Application.Features.Practitioners.Commands.AddScheduleBreak
{
    public sealed class AddScheduleBreakCommandValidator : AbstractValidator<AddSchedulelBreakCommand>
    {
        public AddScheduleBreakCommandValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("Schedule ID is required.");

            RuleFor(x => x.BreakStart)
                .NotEmpty().WithMessage("Break start time is required.")
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("Break start time must be in a valid format (e.g., HH:mm).");

            RuleFor(x => x.BreakEnd)
                .NotEmpty().WithMessage("Break end time is required.")
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("Break end time must be in a valid format (e.g., HH:mm).");
        }
    }
}
