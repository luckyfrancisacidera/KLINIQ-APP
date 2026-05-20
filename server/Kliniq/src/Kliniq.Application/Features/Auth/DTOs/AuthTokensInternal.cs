using Kliniq.Application.Features.Auth.Dto;

namespace Kliniq.Application.Features.Auth.DTOs
{
    public class AuthTokensInternal
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public AuthResponseDto Response { get; set; } = default!;
    }
}
