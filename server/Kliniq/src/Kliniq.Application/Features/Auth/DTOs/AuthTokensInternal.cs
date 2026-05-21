using Kliniq.Application.Features.Auth.Dto;

namespace Kliniq.Application.Features.Auth.DTOs
{
    public record AuthTokensInternal
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public AuthResponseDto Response { get; init; } = default!;
    }
}
