using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task AddAsync(Patient patient, CancellationToken cancellationToken);
        Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
