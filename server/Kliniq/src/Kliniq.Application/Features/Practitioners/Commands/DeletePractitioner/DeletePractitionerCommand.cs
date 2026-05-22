using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Practitioners.Commands.DeletePractitioner
{
    public sealed record DeletePractitionerCommand(Guid PractitionerId): IRequest<Result>;
}
