namespace Kliniq.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResult> RegisterAsync(
            string email,
            string password,
            string role,
            CancellationToken cancellationToken);

        Task<AuthServiceResult> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken);

        Task<AuthServiceResult> RefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken);

        Task RevokeRefreshTokenAsync(
            string userId,
            CancellationToken cancellationToken);

        Task SaveRefreshTokenAsync(
            string userId,
            string refreshTokenHash,
            CancellationToken cancellationToken);
    }

    public class AuthServiceResult
    {
        public string UserId { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public bool Succeeded { get; init; } = true;
    }
}