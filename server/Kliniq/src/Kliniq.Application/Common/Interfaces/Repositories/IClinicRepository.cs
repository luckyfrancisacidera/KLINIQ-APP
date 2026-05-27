using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IClinicRepository
    {
        Task AddAsync(Clinic clinic, CancellationToken cancellationToken);
        Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
