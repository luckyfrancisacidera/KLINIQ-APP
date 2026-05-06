namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public class RegisterPractitionerAccountDto
    {

        public string InvitationToken { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
    }
}
