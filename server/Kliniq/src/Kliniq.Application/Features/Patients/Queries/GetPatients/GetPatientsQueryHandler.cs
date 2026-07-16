using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Application.Features.Patients.Mappings;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Queries.GetPatients
{
    public sealed class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, Result<PagedResult<PatientDto>>>
    {
        private readonly IPatientRepository _patientRepository;
        public GetPatientsQueryHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task<Result<PagedResult<PatientDto>>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
        {
            var paged = await _patientRepository.GetAllAsync(request.Search, request.Page, request.PageSize, cancellationToken);

            var result = new PagedResult<PatientDto>(
                paged.Items.Select(p => p.ToDto()).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize
            );

            return Result.Success(result);
        }
    }
}
