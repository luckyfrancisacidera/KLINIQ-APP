using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Commands.BookAppointment
{
    public sealed record BookAppointmentCommand(
     Guid ScheduleId,
     DateOnly AppointmentDate,
     TimeOnly SlotTime,
     string? Reason
    ) : IRequest<Result<AppointmentDto>>
    {
        public Guid UserId { get; init; }
    }


}
