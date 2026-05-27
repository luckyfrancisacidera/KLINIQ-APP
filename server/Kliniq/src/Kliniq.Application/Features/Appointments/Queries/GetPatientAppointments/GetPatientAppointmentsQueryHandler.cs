using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Appointments.DTOs;
using Kliniq.Application.Features.Appointments.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using MediatR;

namespace Kliniq.Application.Features.Appointments.Queries.GetPatientAppointments
{
    public sealed class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, Result<PagedResult<AppointmentDto>>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;

        public GetPatientAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
        }

        public async Task<Result<PagedResult<AppointmentDto>>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _patientRepository.ExistByIdAsync(request.PatientId, cancellationToken);

            if(!exists)
                return Result.Failure<PagedResult<AppointmentDto>>(Error.NotFound("Patient.NotFound",$"Patient '{request.PatientId}' not found"));

            var paged = await _appointmentRepository.GetByPatientIdAsync(request.PatientId, request.Page, request.PageSize, cancellationToken);

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
