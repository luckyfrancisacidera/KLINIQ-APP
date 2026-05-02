using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IPractitionerRepository
    {
        Task AddAsync(Practitioner practitioner, CancellationToken cancellationToken);
        Task<Practitioner?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Practitioner?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
