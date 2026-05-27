using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Queries.GetPatient
{
    public sealed record GetPatientQuery(Guid PatientId) : IRequest<Result<PatientDto>>;
}
