using Kliniq.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Kliniq.Infrastructure.Services
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration) => _configuration = configuration;

        public string GenerateAccessToken(string userId, string email, string role)
        {
            var keyValue = GetRequiredSetting("JwtSettings:Key");
            if (Encoding.UTF8.GetByteCount(keyValue) < 32)
                throw new InvalidOperationException("JwtSettings:Key must be at least 32 bytes.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: GetRequiredSetting("JwtSettings:Issuer"),
                audience: GetRequiredSetting("JwtSettings:Audience"),
                claims: claims,
                expires: GetAccessTokenExpiry(),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetAccessTokenExpiry()
        {
            var raw = _configuration["JwtSettings:ExpiryMinutes"];
            return DateTime.UtcNow.AddMinutes(int.TryParse(raw, out var minutes) ? Math.Clamp(minutes, 5, 120) : 15);
        }

        public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public string HashRefreshToken(string refreshToken)
            => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

        private string GetRequiredSetting(string key)
            => !string.IsNullOrWhiteSpace(_configuration[key])
                ? _configuration[key]!
                : throw new InvalidOperationException($"Required configuration '{key}' is missing.");
    }
}
