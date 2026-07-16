using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment, CancellationToken cancellationToken);

        // For QUERIES
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResult<Appointment>> GetByPatientIdAsync(Guid patientId, string? status, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<Appointment>> GetByPractitionerIdAsync(Guid practitionerId, string? status, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<Appointment>> GetByPractitionerInRangeAsync(Guid practitionerId, DateTime from, DateTime to, CancellationToken cancellationToken);
        Task<bool> HasConflictAsync(Guid practitionerId, DateTime scheduledAt, int durationMinutes, Guid? excludeId, CancellationToken cancellationToken);

        // For COMMANDS
        Task<Appointment?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken);

        void Update(Appointment appointment);
    }
}
