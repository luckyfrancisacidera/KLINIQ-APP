using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Common.Models;
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
        public async Task<PagedResult<Practitioner>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = Pagination.Normalize(page, pageSize);
            var query = _context.Practitioners.AsNoTracking().Include(p => p.Clinic);
            var total = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(p => p.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PagedResult<Practitioner>(items, total, page, pageSize);
        }

        public async Task<PagedResult<Practitioner>> SearchAsync(string? search, string? specialization, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = Pagination.Normalize(page, pageSize);
            var query = _context.Practitioners.AsNoTracking().Include(p => p.Clinic).AsQueryable();

            if(!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p => (p.Name.FirstName + " " + p.Name.LastName).ToLower().Contains(term) || p.LicenseNumber.ToLower().Contains(term));
            }

            if(!string.IsNullOrWhiteSpace(specialization))
            {
                var spec = specialization.Trim().ToLower();
                query = query.Where(p => EF.Property<string>(p, "_specialization").ToLower().Contains(spec));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(p => p.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new PagedResult<Practitioner>(items, total, page, pageSize);
        }

        //For Command Methods

        public async Task<Practitioner?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Practitioners.Include(p => p.Clinic).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);


        // For checking if practitioner exists
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Practitioners.AnyAsync(p => p.Id == id, cancellationToken);

        public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.Practitioners.AnyAsync(p => p.UserId == userId, cancellationToken);

        //For Update and Delete
        public void Update(Practitioner practitioner)
            => _context.Practitioners.Update(practitioner);
        public void Delete(Practitioner practitioner)
            => _context.Practitioners.Remove(practitioner);


    }
}
