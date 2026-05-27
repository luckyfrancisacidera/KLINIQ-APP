using FluentValidation;

namespace Kliniq.Application.Features.Appointments.Commands.BookAppointment
{
    public sealed class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
    {
        public BookAppointmentCommandValidator()
        {
            RuleFor(x => x.ScheduleId)
                .NotEmpty().WithMessage("ScheduleId is required.");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty().WithMessage("AppointmentDate is required.")
                .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                .WithMessage("AppointmentDate cannot be in the past.");

            RuleFor(x => x.SlotTime)
                .NotEmpty().WithMessage("SlotTime is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Authenticated user could not be resolved.");
        }
    }
}