using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Persistence.Repositories
{
    public sealed class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context) => _context = context;
        

        public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
            => await _context.Appointments.AddAsync(appointment, cancellationToken);

        //For Query Methods
        public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        public async Task<PagedResult<Appointment>> GetByPatientIdAsync(Guid patientId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Appointments.AsNoTracking()
                .Where(a => a.PatientId == patientId);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(a => a.ScheduledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Appointment>(items, total, page, pageSize);
        }

        public async Task<PagedResult<Appointment>> GetByPractitionerIdAsync(Guid practitionerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Appointments.AsNoTracking()
                .Where(a => a.PractitionerId == practitionerId);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(a => a.ScheduledAt)
                .Skip((page -1 ) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Appointment>(items, total, page, pageSize);

        }

        public async Task<IReadOnlyList<Appointment>> GetByPractitionerInRangeAsync(Guid practitionerId, DateTime from, DateTime to, CancellationToken cancellationToken)
            => await _context.Appointments
                .AsNoTracking()
                .Where(a => a.PractitionerId == practitionerId &&
                            a.ScheduledAt >= from &&
                            a.ScheduledAt <= to)
                .ToListAsync(cancellationToken);

        //For Command Methods
        public async Task<Appointment?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        
        //For checking conflicts

        public async Task<bool> HasConflictAsync(Guid practitionerId, DateTime scheduledAt, int durationMinutes, Guid? excludeId, CancellationToken cancellationToken)
        {
            var proposedEnd = scheduledAt.AddMinutes(durationMinutes);

            var candidates = await _context.Appointments
             .Where(a =>
                 a.PractitionerId == practitionerId &&
                 a.Status != AppointmentStatus.Cancelled &&
                 (excludeId == null || a.Id != excludeId) &&
                 a.ScheduledAt < proposedEnd &&
                 a.ScheduledAt >= scheduledAt.Date)   
             .Select(a => new { a.ScheduledAt, DurationMinutes = (int)a.Duration.TotalMinutes })
             .ToListAsync(cancellationToken);

            return candidates.Any(a => a.ScheduledAt.AddMinutes(a.DurationMinutes) > scheduledAt);

        }

        public void Update(Appointment appointment)
            => _context.Appointments.Update(appointment);
    }
}
