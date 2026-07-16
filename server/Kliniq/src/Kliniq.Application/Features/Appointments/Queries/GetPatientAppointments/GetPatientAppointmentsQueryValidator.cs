using FluentValidation;
using Kliniq.Domain.Enums;

namespace Kliniq.Application.Features.Appointments.Queries.GetPatientAppointments
{
    public sealed class GetPatientAppointmentsQueryValidator : AbstractValidator<GetPatientAppointmentsQuery>
    {
        public GetPatientAppointmentsQueryValidator()
        {
            RuleFor(x => x.PatientId).NotEmpty();
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrWhiteSpace(status) || Enum.TryParse<AppointmentStatus>(status, true, out _))
                .WithMessage("Status is invalid.");

            RuleFor(x => x)
                .Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue || x.DateFrom <= x.DateTo)
                .WithMessage("DateFrom must be earlier than or equal to DateTo.");
        }
    }
}
