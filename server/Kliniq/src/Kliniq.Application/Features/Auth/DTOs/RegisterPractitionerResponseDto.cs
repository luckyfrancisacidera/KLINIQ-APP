namespace Kliniq.Application.Features.Auth.DTOs
{
    public record RegisterPractitionerResponseDto
    {
        public string Email { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}
