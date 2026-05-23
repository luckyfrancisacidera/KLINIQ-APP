using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Persistence.Repositories
{
    public class PractitionerRepository : IPractitionerRepository 
    {
        private readonly AppDbContext _context;

        public PractitionerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Practitioner practitioner, CancellationToken cancellationToken)
            => await _context.Practitioners.AddAsync(practitioner, cancellationToken);

        // For Query Methods
        public async Task<Practitioner?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Practitioners.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<Practitioner?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
          => await _context.Practitioners.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        public async Task<Practitioner?> GetByIdWithSchedulesAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Practitioners.AsNoTracking()
                .Include(p => p.Schedules).ThenInclude(s => s.Breaks)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        //For Command Methods
        public async Task<PagedResult<Practitioner>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Practitioners.AsNoTracking();
            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PagedResult<Practitioner>(items, total, page, pageSize);
        }

        public async Task<Practitioner?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Practitioners.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Practitioners.AnyAsync(p => p.Id == id, cancellationToken);

        public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.Practitioners.AnyAsync(p => p.UserId == userId, cancellationToken);

        public void Update(Practitioner practitioner)
        {
            throw new NotImplementedException();
        }
        public void Delete(Practitioner practitioner)
        {
            throw new NotImplementedException();
        }
    }
}
