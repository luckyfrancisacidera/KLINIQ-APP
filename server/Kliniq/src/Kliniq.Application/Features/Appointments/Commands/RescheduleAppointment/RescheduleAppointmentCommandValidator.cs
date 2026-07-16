using FluentValidation;

namespace Kliniq.Application.Features.Appointments.Commands.RescheduleAppointment
{
    public sealed class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
    {
        public RescheduleAppointmentCommandValidator()
        {
            RuleFor(x => x.AppointmentId).NotEmpty();
            RuleFor(x => x.ScheduleId).NotEmpty();
            RuleFor(x => x.AppointmentDate)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                .WithMessage("Appointment date cannot be in the past.");
        }
    }
}
