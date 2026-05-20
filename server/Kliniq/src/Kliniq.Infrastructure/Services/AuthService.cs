using Kliniq.Application.Common.Interfaces;
using Kliniq.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

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

        public async Task<AuthServiceResult> RegisterAsync(
            string email,
            string password,
            string role,
            CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
                throw new InvalidOperationException("Email is already registered");

            var user = new AppUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, role);

            return new AuthServiceResult
            {
                UserId = user.Id,  
                Email = user.Email!,
                Role = role
            };
        }

        public async Task<AuthServiceResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new UnauthorizedAccessException("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            return new AuthServiceResult
            {
                UserId = user.Id,       
                Email = user.Email!,
                Role = role
            };
        }

        public async Task<AuthServiceResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var tokenHash =  _jwtTokenService.HashRefreshToken(refreshToken);

            var user = _userManager.Users.FirstOrDefault(u => u.RefreshTokenHash == tokenHash);

            if (user is null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if(user.RefreshTokenExpiresAtUtc is null || user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired. Please log in again");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            return new AuthServiceResult
            {
                UserId = user.Id,
                Email = user.Email!,
                Role = role
            };
        }

        public async Task RevokeTokenAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found");

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAtUtc = null;

            await _userManager.UpdateAsync(user);
        }


        public async Task SaveRefreshTokenAsync(string userId, string refreshTokenHash, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found");

            user.RefreshTokenHash = refreshTokenHash;
            user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime);

            await _userManager.UpdateAsync(user);
        }
    }
}