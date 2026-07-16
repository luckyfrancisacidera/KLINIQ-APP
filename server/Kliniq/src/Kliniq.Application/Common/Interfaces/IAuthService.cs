using Kliniq.Domain.Common;

namespace Kliniq.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthServiceResult>> RegisterAsync(
            string email,
            string password,
            string role,
            CancellationToken cancellationToken);

        Task<Result<AuthServiceResult>> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken);

        Task<Result<AuthServiceResult>> RefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken);

        Task<Result> RevokeTokenAsync(
            string userId,
            CancellationToken cancellationToken);

        Task<Result> SaveRefreshTokenAsync(
            string userId,
            string refreshTokenHash,
            CancellationToken cancellationToken);

        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

        Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken);

        Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);

        Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
    }

    public class AuthServiceResult
    {
        public string UserId { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public bool Succeeded { get; init; } = true;
    }
}