namespace Kliniq.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<(string userId, string email)> RegisterAsync(string email, string password, string role, CancellationToken cancellationToken);
        Task<(string userId, string email)> LoginAsync(string email, string password, CancellationToken cancellationToken);
    }
}
