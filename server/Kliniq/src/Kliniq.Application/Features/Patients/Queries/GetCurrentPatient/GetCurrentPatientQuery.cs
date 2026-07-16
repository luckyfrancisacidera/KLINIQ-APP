using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Queries.GetCurrentPatient
{
    public sealed record GetCurrentPatientQuery(Guid UserId) : IRequest<Result<PatientDto>>;
}
