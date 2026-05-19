using Kliniq.Domain.Enums;
using Kliniq.Infrastructure.Identity;
using Kliniq.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kliniq.Infrastructure.Persistence.Seeder
{

}
public class IdentitySeeder
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
        var roles = Enum.GetNames<UserRole>();

        foreach (var role in roles)
        {
            if (await _roleManager.RoleExistsAsync(role))
                continue;

            var result = await _roleManager.CreateAsync(new IdentityRole(role));

            if (result.Succeeded)
                _logger.LogInformation("Role '{Role}' created", role);
            else
                _logger.LogError("Failed to create role '{Role}': {Errors}",
                    role, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private async Task SeedAdminAsync()
    {
        if (string.IsNullOrWhiteSpace(_settings.AdminEmail) ||
            string.IsNullOrWhiteSpace(_settings.AdminPassword))
        {
            _logger.LogWarning(
                "Admin seed skipped — SeedSettings:AdminEmail or SeedSettings:AdminPassword not set.");
            return;
        }

        var existing = await _userManager.FindByEmailAsync(_settings.AdminEmail);
        if (existing is not null)
        {
            _logger.LogInformation("Admin user already exists, skipping seed");
            return;
        }

        var admin = new AppUser
        {
            UserName = _settings.AdminEmail,
            Email = _settings.AdminEmail,
            EmailConfirmed = true,
            Role = UserRole.Admin
        };

        var result = await _userManager.CreateAsync(admin, _settings.AdminPassword);

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to create admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await _userManager.AddToRoleAsync(admin, nameof(UserRole.Admin));
        _logger.LogInformation("Admin user seeded successfully");
    }
}