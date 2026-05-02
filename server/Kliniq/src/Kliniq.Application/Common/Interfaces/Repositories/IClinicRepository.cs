using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IClinicRepository
    {
        Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
