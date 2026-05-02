using Kliniq.Application.Common.Interfaces.Repositories;
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
            => await _context.Practioners.AddAsync(practitioner, cancellationToken);

        public async Task<Practitioner?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Practioners.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<Practitioner?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.Practioners.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }
}
