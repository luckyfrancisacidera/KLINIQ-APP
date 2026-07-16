using Kliniq.Application.Common.Interfaces;
using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using Kliniq.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        public AuthService(UserManager<AppUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<AuthServiceResult>> RegisterAsync(string email, string password, string role, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.Trim();
            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser is not null)
                return Result.Failure<AuthServiceResult>(Error.Conflict("Auth.EmailTaken", "An account with this email already exists."));

            if (!Enum.TryParse<UserRole>(role, true, out var userRole))
                return Result.Failure<AuthServiceResult>(Error.Validation("Auth.InvalidRole", "The requested account role is invalid."));

            var user = new AppUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                Role = userRole
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Result.Failure<AuthServiceResult>(Error.Validation(
                    "Auth.RegistrationFailed",
                    string.Join(" ", result.Errors.Select(error => error.Description))));

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Result.Failure<AuthServiceResult>(Error.Failure("Auth.RoleAssignmentFailed", "The account could not be created."));
            }

            return Result.Success(ToResult(user, role));
        }

        public async Task<Result<AuthServiceResult>> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim());
            if (user is null)
                return InvalidCredentials();

            if (await _userManager.IsLockedOutAsync(user))
                return InvalidCredentials();

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                return InvalidCredentials();
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success(ToResult(user, roles.FirstOrDefault() ?? user.Role.ToString()));
        }

        public async Task<Result<AuthServiceResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var tokenHash = _jwtTokenService.HashRefreshToken(refreshToken);
            var user = await _userManager.Users.SingleOrDefaultAsync(
                candidate => candidate.RefreshTokenHash == tokenHash,
                cancellationToken);

            if (user is null || user.RefreshTokenExpiresAtUtc is null || user.RefreshTokenExpiresAtUtc <= DateTime.UtcNow)
                return Result.Failure<AuthServiceResult>(Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid or expired refresh token."));

            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success(ToResult(user, roles.FirstOrDefault() ?? user.Role.ToString()));
        }

        public async Task<Result> RevokeTokenAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(Error.NotFound("Auth.UserNotFound", "User not found."));

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAtUtc = null;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(Error.Failure("Auth.RevokeFailed", "The session could not be revoked."));
        }

        public async Task<Result> SaveRefreshTokenAsync(string userId, string refreshTokenHash, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(Error.NotFound("Auth.UserNotFound", "User not found."));

            user.RefreshTokenHash = refreshTokenHash;
            user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(Error.Failure("Auth.RefreshTokenSaveFailed", "The session could not be created."));
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
            => await _userManager.FindByEmailAsync(email.Trim()) is not null;

        public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim());
            return user is null ? null : await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim());
            if (user is null)
                return Result.Failure(Error.Validation("Auth.InvalidReset", "The password reset request is invalid or expired."));

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return Result.Failure(Error.Validation("Auth.InvalidReset", string.Join(" ", result.Errors.Select(error => error.Description))));

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAtUtc = null;
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(Error.NotFound("Auth.UserNotFound", "User not found."));

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return Result.Failure(Error.Validation("Auth.ChangePasswordFailed", string.Join(" ", result.Errors.Select(error => error.Description))));

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAtUtc = null;
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }

        private static AuthServiceResult ToResult(AppUser user, string role) => new()
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Role = role
        };

        private static Result<AuthServiceResult> InvalidCredentials()
            => Result.Failure<AuthServiceResult>(Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));
    }
}
