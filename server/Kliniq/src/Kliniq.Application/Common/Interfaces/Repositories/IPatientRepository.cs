using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task AddAsync(Patient patient, CancellationToken cancellationToken);

        //For QUERIES
        Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<PagedResult<Patient>> GetAllAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);

        //For COMMANDS
        Task<Patient?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken);

        //Fpr checking if patient exists
        Task<bool> ExistByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        void Update(Patient patient);
        void Delete(Patient patient);

    }
}
