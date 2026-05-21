namespace Kliniq.Application.Features.Auth.Dto
{
    public record AuthResponseDto
    {                                                        
      
        public DateTime AccessTokenExpiresAtUtc { get; init; }
        public string UserId { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;

    #if DEBUG
        public string? DevAccessToken { get; init; }
    #endif
    }
}
