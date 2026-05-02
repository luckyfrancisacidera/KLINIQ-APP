using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Domain.Enums;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        //account
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        //patient profile
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
    }
}
