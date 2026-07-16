using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Common.Models;
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

        public async Task<PagedResult<Appointment>> GetByPatientIdAsync(Guid patientId, string? status, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = Pagination.Normalize(page, pageSize);
            var query = ApplyFilters(_context.Appointments.AsNoTracking().Where(a => a.PatientId == patientId), status, dateFrom, dateTo);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(a => a.ScheduledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Appointment>(items, total, page, pageSize);
        }

        public async Task<PagedResult<Appointment>> GetByPractitionerIdAsync(Guid practitionerId, string? status, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = Pagination.Normalize(page, pageSize);
            var query = ApplyFilters(_context.Appointments.AsNoTracking().Where(a => a.PractitionerId == practitionerId), status, dateFrom, dateTo);

            var total = await query.CountAsync(cancellationToken);
            var ordered = string.Equals(status, AppointmentStatus.InQueue.ToString(), StringComparison.OrdinalIgnoreCase)
                ? query.OrderBy(a => a.QueuedAtUtc).ThenBy(a => a.ScheduledAt)
                : query.OrderByDescending(a => a.ScheduledAt);

            var items = await ordered
                .Skip((page - 1) * pageSize)
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

            return await _context.Appointments
                .AsNoTracking()
                .AnyAsync(a =>
                    a.PractitionerId == practitionerId &&
                    a.Status != AppointmentStatus.Cancelled &&
                    (!excludeId.HasValue || a.Id != excludeId.Value) &&
                    a.ScheduledAt < proposedEnd &&
                    a.EndTime > scheduledAt,
                    cancellationToken);
        }

        private static IQueryable<Appointment> ApplyFilters(
            IQueryable<Appointment> query,
            string? status,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AppointmentStatus>(status, true, out var parsedStatus))
                query = query.Where(appointment => appointment.Status == parsedStatus);

            if (dateFrom.HasValue)
                query = query.Where(appointment => appointment.ScheduledAt >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(appointment => appointment.ScheduledAt < dateTo.Value.AddDays(1));

            return query;
        }

        public void Update(Appointment appointment)
            => _context.Appointments.Update(appointment);
    }
}
