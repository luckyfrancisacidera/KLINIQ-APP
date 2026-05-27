using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Queries.GetPractitionerAppointments
{

    public sealed class GetPractitionerAppointmentsQueryHandler  : IRequestHandler<GetPractitionerAppointmentsQuery, Result<PagedResult<AppointmentDto>>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPractitionerRepository _practitionerRepository;

        public GetPractitionerAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IPractitionerRepository practitionerRepository)
        {
            _appointmentRepository = appointmentRepository;
            _practitionerRepository = practitionerRepository;
        }

        public async Task<Result<PagedResult<AppointmentDto>>> Handle(GetPractitionerAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _practitionerRepository.ExistsAsync(request.PractitionerId, cancellationToken);
            
            if(!exists)
                return Result.Failure<PagedResult<AppointmentDto>>(Error.NotFound("Practitioner.NotFound", $"Practitioner with id {request.PractitionerId} was not found."));

            var paged = await _appointmentRepository.GetByPractitionerIdAsync(request.PractitionerId, request.Page, request.PageSize, cancellationToken);

            var mapped = new PagedResult<AppointmentDto>(
                paged.Items.Select(a => a.ToDto()).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize
            );

            return Result.Success(mapped);
        }
    }
}
