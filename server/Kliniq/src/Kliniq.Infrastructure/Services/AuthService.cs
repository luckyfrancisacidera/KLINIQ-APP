using Kliniq.Application.Common.Interfaces;
using Kliniq.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Kliniq.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;

        public AuthService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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

        public async Task<AuthServiceResult> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new UnauthorizedAccessException("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthServiceResult
            {
                UserId = user.Id,       
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty
            };
        }
    }
}