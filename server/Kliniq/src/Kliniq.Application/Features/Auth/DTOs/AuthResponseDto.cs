namespace Kliniq.Application.Features.Auth.Dto
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
