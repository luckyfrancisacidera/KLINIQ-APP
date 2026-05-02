using Kliniq.Application.Common.Interfaces.Repositories;
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
        
        public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.Patients.AnyAsync(p => p.UserId == userId, cancellationToken);

    }
}
