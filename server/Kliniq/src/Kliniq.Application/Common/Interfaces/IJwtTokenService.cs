namespace Kliniq.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(string userId, string email, string role);
        DateTime GetAccessTokenExpiry();
        
        string GenerateRefreshToken();

        string HashRefreshToken(string refreshToken);
    }
}
