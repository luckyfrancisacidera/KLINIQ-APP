using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Persistence.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly AppDbContext _context;

        public ClinicRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Clinics
                .Include(c => c.Practioners)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
