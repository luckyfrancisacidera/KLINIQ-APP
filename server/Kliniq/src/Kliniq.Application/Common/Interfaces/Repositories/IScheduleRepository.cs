using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        Task AddAsync(Schedule schedule, CancellationToken cancellationToken);

        // For QUERIES
        Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Schedule?> GetByIdWithBreaksAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Schedule>> GetByPractitionerIdAsync(Guid practitionerId, CancellationToken cancellationToken);

        //For COMMANDS
        Task<Schedule?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken);
        Task<Schedule?> GetByIdWithBreaksTrackedAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> HasTimeOverlapAsync(Guid practitionerId, int day, TimeOnly newStart, TimeOnly newEnd, Guid? excludeId, CancellationToken cancellationToken);

        void Update(Schedule schedule);
        void Delete(Schedule schedule);
    }
}
