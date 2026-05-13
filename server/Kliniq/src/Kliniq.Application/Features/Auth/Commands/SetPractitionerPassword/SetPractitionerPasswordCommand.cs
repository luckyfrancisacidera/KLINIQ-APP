
using Kliniq.Application.Features.Auth.DTOs;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword
{
    public class SetPractitionerPasswordCommand : IRequest<RegisterPractitionerResponseDto>
    {
        public string InvitationToken { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
