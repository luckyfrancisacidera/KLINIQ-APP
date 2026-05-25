using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.BookAppointment
{
    public sealed record BookAppointmentCommand(
        Guid PatientId,
        Guid PractitionerId,
        Guid ClinicId,
        DateTime ScheduledAt,
        string? Reason
     ) : IRequest<Result<AppointmentDto>>;
}
