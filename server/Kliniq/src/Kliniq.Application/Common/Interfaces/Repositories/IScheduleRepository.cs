using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        Task AddAsync(Schedule schedule, CancellationToken cancellationToken);
        Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Schedule?> GetByIdWithBreaksAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Schedule>> GetByPractitionerIdAsync(Guid practitionerId, CancellationToken cancellationToken);
        Task<bool> HasOverlappingScheduleAsync(Guid practitionerId, int day, Guid? excludeId, CancellationToken cancellationToken);
        void Update(Schedule schedule);
        void Delete(Schedule schedule);
    }
}
