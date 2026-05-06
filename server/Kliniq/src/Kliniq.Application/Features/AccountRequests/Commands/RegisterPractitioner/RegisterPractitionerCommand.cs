using Kliniq.Application.Features.AccountRequests.DTOs;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.RegisterPractitioner
{
    public class RegisterPractitionerCommand : IRequest<RegisterPractitionerAccountDto>
    {
        public string InvitationToken { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
    }
}
