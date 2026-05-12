namespace Kliniq.Api.Models.Requests
{
    public class SetPasswordRequest
    {
        public string InvitationToken { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
    }
}