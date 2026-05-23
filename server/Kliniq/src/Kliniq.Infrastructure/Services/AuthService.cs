using Kliniq.Application.Common.Interfaces;
using Kliniq.Domain.Common;
using Kliniq.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Kliniq.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        public AuthService(UserManager<AppUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<AuthServiceResult>> RegisterAsync(
            string email,
            string password,
            string role,
            CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
                return Result.Failure<AuthServiceResult>(Error.Conflict("Auth.EmailTaken", "An account with this email already exists"));

            var user = new AppUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Result.Failure<AuthServiceResult>(Error.Failure("Auth.RegistrationFailed", string.Join(", ", result.Errors.Select(e => e.Description))));

            await _userManager.AddToRoleAsync(user, role);

            return Result.Success(new AuthServiceResult
            {
                UserId = user.Id,  
                Email = user.Email!,
                Role = role
            });
        }

        public async Task<Result<AuthServiceResult>> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, password))
                return Result.Failure<AuthServiceResult>(Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password"));

            var roles = await _userManager.GetRolesAsync(user);

            return Result.Success(new AuthServiceResult
            {
                UserId = user.Id,       
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty    
            });
        }

        public async Task<Result<AuthServiceResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var tokenHash =  _jwtTokenService.HashRefreshToken(refreshToken);

            var user = _userManager.Users.FirstOrDefault(u => u.RefreshTokenHash == tokenHash);

            if (user is null)
                return Result.Failure<AuthServiceResult>(Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid refresh token"));

            if (user.RefreshTokenExpiresAtUtc is null || user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
                return Result.Failure<AuthServiceResult>(Error.Unauthorized("Auth.ExpiredRefreshToken", "Refresh token has expired"));

            var roles = await _userManager.GetRolesAsync(user);

            return Result.Success(new AuthServiceResult
            {
                UserId = user.Id,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty
            });
        }

        public async Task<Result> RevokeTokenAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure(Error.NotFound("Auth.UserNotFound", "User not found"));

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAtUtc = null;

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }


        public async Task<Result> SaveRefreshTokenAsync(string userId, string refreshTokenHash, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user is null)
                return Result.Failure(Error.NotFound("Auth.UserNotFound", "User not found"));

            user.RefreshTokenHash = refreshTokenHash;
            user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime);

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user is not null;
        }
    }
}