using Kliniq.Application.Common.Interfaces;
using Kliniq.Domain.Enums;
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
        public async Task<(string userId, string email)> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new InvalidOperationException("Invalid email or password.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

            if(!isPasswordValid)
                throw new InvalidOperationException("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            return(user.Id ?? throw new InvalidOperationException("User ID cannot be null."), user.Email ?? throw new InvalidOperationException("User email cannot be null."));
        }

        public async Task<(string userId, string email)> RegisterAsync(string email, string password, string role, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if(existingUser is not null)
                throw new InvalidOperationException("User with the same email already exists.");

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                Role = Enum.Parse<UserRole>(role, true)
            };

            var result = await _userManager.CreateAsync(user, password);
            if(!result.Succeeded)
                throw new InvalidOperationException(string.Join(",", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, role);

            return (user.Id ?? throw new InvalidOperationException("User ID cannot be null."), user.Email ?? throw new InvalidOperationException("User email cannot be null."));
        }
    }
}
