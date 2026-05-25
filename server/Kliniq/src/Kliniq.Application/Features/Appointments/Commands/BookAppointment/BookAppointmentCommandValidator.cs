using FluentValidation;

namespace Kliniq.Application.Features.Appointments.Commands.BookAppointment
{
    public sealed class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
    {
        public BookAppointmentCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("PatientId is required.");

            RuleFor(x => x.PractitionerId)
                .NotEmpty().WithMessage("PractitionerId is required.");

            RuleFor(x => x.ClinicId)
                .NotEmpty().WithMessage("ClinicId is required.");

            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("ScheduledAt must be in the future.");

            RuleFor(x => x.Reason).MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
