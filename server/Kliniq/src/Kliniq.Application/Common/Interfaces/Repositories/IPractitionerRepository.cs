using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IPractitionerRepository
    {
        Task AddAsync(Practitioner practitioner, CancellationToken cancellationToken);

        // For QUERIES
        Task<Practitioner?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Practitioner?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Practitioner?> GetByIdWithSchedulesAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResult<Practitioner>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<Practitioner>> SearchAsync(string? search, string? specialization, int page, int pageSize, CancellationToken cancellationToken);

        // For COMMANDS
        Task<Practitioner?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken);

        //For checking if practitioner exists
        Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

        void Update(Practitioner practitioner);
        void Delete(Practitioner practitioner);
    }
}