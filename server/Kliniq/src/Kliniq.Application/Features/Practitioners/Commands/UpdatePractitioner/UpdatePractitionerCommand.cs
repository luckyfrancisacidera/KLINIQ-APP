using Kliniq.Application.Features.Practitioners.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.UpdatePractitioner
{
    public sealed record UpdatePractitionerCommand
    (
        Guid PractitionerId,
        string FirstName,
        string LastName,
        string Specialization
    ) : IRequest<Result<PractitionerDto>>;
}
