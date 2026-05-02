using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IAccountRequestRepository
    {
        Task AddAsync(AccountRequest accountRequest, CancellationToken cancellationToken);
        Task<AccountRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsPendingEmailAsync(string email, CancellationToken cancellationToken);
        Task<AccountRequest?> GetApprovedByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
