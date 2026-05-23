using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.Sig;

namespace Kliniq.Infrastructure.Persistence.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;
        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Schedule schedule, CancellationToken cancellationToken)
            => await _context.Schedules.AddAsync(schedule, cancellationToken);
        
        //For Query Methods
        public async Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Schedules.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<Schedule?> GetByIdWithBreaksAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Schedules.AsNoTracking().Include(s => s.Breaks).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<IReadOnlyList<Schedule>> GetByPractitionerIdAsync(Guid practitionerId, CancellationToken cancellationToken)
            => await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Breaks)
                .Where(s => s.PractitionerId == practitionerId)
                .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
                .ToListAsync(cancellationToken);

        //For Command Methods
        public async Task<Schedule?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<Schedule?> GetByIdWithBreaksTrackedAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Schedules.Include(s => s.Breaks).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<bool> HasTimeOverlapAsync(Guid practitionerId, int day, TimeOnly newStart, TimeOnly newEnd, Guid? excludeId, CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .AnyAsync(s => s.PractitionerId == practitionerId 
                    && (int)s.Day == day 
                    && (excludeId == null || s.Id != excludeId)
                    && s.StartTime < newEnd && s.EndTime > newStart, cancellationToken);
        }

        public void Update(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
        }

        public void Delete(Schedule schedule)
        {
            _context.Schedules.Remove(schedule);
        }

    }
}
