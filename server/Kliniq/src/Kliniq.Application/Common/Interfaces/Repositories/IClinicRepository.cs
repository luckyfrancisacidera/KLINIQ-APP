using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IClinicRepository
    {
        Task AddAsync(Clinic clinic, CancellationToken cancellationToken);
        Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResult<Clinic>> SearchAsync(
            string? search,
            string? specialization,
            double? latitude,
            double? longitude,
            double? radiusKm,
            string? sortBy,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
