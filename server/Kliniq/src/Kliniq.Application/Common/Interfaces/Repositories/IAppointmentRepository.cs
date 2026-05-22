using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        Task AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken);
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResult<Appointment>> GetByPatientIdAsync(Guid patientId, int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<Appointment>> GetByPractitionerIdAsync(Guid practitionerId, int page, int pageSize, CancellationToken cancellationToken);
        Task<bool> HasConflictAsync(Guid practitionerId, DateTime scheduledAt, int durationMinutes, Guid? excludeId, CancellationToken cancellationToken);
        void Delete(Appointment appointment);
    }
}
