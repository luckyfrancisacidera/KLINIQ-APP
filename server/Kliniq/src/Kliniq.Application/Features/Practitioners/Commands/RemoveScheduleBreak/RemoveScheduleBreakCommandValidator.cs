using FluentValidation;

namespace Kliniq.Application.Features.Practitioners.Commands.RemoveScheduleBreak
{
    public sealed class RemoveScheduleBreakCommandValidator : AbstractValidator<RemoveScheduleBreakCommand>
    {
        public RemoveScheduleBreakCommandValidator()
        {
            RuleFor(x => x.ScheduleId)
                .NotEmpty().WithMessage("Schedule ID is required.");

            RuleFor(x => x.BreakId)
                .NotEmpty().WithMessage("Break ID is required.");
        }
    }
}
