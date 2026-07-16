using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.RescheduleAppointment
{
    public sealed record RescheduleAppointmentCommand(
        Guid AppointmentId,
        Guid ScheduleId,
        DateOnly AppointmentDate,
        TimeOnly SlotTime) : IRequest<Result<AppointmentDto>>;
}
