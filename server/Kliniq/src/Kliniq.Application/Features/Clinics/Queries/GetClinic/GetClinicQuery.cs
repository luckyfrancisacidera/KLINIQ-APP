using Kliniq.Application.Features.Clinics.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Clinics.Queries.GetClinic
{
    public sealed record GetClinicQuery(Guid ClinicId) : IRequest<Result<ClinicDetailDto>>;
}
