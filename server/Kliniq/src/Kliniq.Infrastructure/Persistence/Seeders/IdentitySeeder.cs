using Kliniq.Domain.Enums;
using Kliniq.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kliniq.Infrastructure.Persistence.Seeders
{
    public sealed class IdentitySeeder
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentitySeeder> _logger;
        private readonly SeedSettings _settings;

        public IdentitySeeder(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentitySeeder> logger,
            IOptions<SeedSettings> settings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminAsync();
        }

        private async Task SeedRolesAsync()
        {
            foreach (var role in Enum.GetNames<UserRole>())
            {
                if (await _roleManager.RoleExistsAsync(role)) continue;
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                    _logger.LogError("Failed to create role {Role}: {Errors}", role, string.Join(", ", result.Errors.Select(error => error.Description)));
            }
        }

        private async Task SeedAdminAsync()
        {
            if (string.IsNullOrWhiteSpace(_settings.AdminEmail) || string.IsNullOrWhiteSpace(_settings.AdminPassword))
            {
                _logger.LogInformation("Admin seed skipped because seed credentials are not configured.");
                return;
            }

            if (await _userManager.FindByEmailAsync(_settings.AdminEmail.Trim()) is not null) return;

            var admin = new AppUser
            {
                UserName = _settings.AdminEmail.Trim(),
                Email = _settings.AdminEmail.Trim(),
                EmailConfirmed = true,
                Role = UserRole.Admin
            };

            var result = await _userManager.CreateAsync(admin, _settings.AdminPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(error => error.Description)));
                return;
            }

            await _userManager.AddToRoleAsync(admin, nameof(UserRole.Admin));
            _logger.LogInformation("Configured administrator account created.");
        }
    }
}
