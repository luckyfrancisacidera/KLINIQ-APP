using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Persistence.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Patient patient, CancellationToken cancellationToken)
            => await _context.Patients.AddAsync(patient, cancellationToken);
        
        // For Query Methods
        public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
           => await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        public async Task<PagedResult<Patient>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Patients.AsNoTracking();

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(p => p.Name.LastName)
                .ThenBy(p => p.Name.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Patient>(items, total, page, pageSize);
        }

        //For Command Methods
        public async Task<Patient?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);


        //For checking if patient exists
        public async Task<bool> ExistByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Patients.AnyAsync(p => p.Id == id, cancellationToken);

        public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.Patients.AnyAsync(p => p.UserId == userId, cancellationToken);

        public void Update(Patient patient)
            => _context.Patients.Update(patient);

        public void Delete(Patient patient)
            => _context.Patients.Remove(patient);
    }
}
