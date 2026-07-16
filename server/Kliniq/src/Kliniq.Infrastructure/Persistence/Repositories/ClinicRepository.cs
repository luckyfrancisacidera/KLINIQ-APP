using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Common.Models;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Persistence.Repositories
{
    public sealed class ClinicRepository : IClinicRepository
    {
        private readonly AppDbContext _context;

        public ClinicRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(Clinic clinic, CancellationToken cancellationToken)
            => await _context.Clinics.AddAsync(clinic, cancellationToken);

        public async Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Clinics
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Practitioners)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task<PagedResult<Clinic>> SearchAsync(
            string? search,
            string? specialization,
            double? latitude,
            double? longitude,
            double? radiusKm,
            string? sortBy,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            (page, pageSize) = Pagination.Normalize(page, pageSize);

            var query = _context.Clinics.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    c.Practitioners.Any(p =>
                        (p.Name.FirstName + " " + p.Name.LastName).ToLower().Contains(term) ||
                        EF.Property<string>(p, "_specialization").ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                var term = specialization.Trim().ToLower();
                query = query.Where(c => c.Practitioners.Any(p =>
                    EF.Property<string>(p, "_specialization").ToLower().Contains(term)));
            }

            if (latitude.HasValue && longitude.HasValue && radiusKm is > 0)
            {
                var boundedRadius = Math.Clamp(radiusKm.Value, 1, 200);
                var latitudeDelta = boundedRadius / 111.32d;
                var longitudeScale = Math.Max(0.15d, Math.Cos(latitude.Value * Math.PI / 180d));
                var longitudeDelta = boundedRadius / (111.32d * longitudeScale);
                var minLatitude = latitude.Value - latitudeDelta;
                var maxLatitude = latitude.Value + latitudeDelta;
                var minLongitude = longitude.Value - longitudeDelta;
                var maxLongitude = longitude.Value + longitudeDelta;

                query = query.Where(c =>
                    c.Location.Latitude >= minLatitude && c.Location.Latitude <= maxLatitude &&
                    c.Location.Longitude >= minLongitude && c.Location.Longitude <= maxLongitude);
            }

            query = sortBy?.Trim().ToLowerInvariant() switch
            {
                "nearest" when latitude.HasValue && longitude.HasValue => query
                    .OrderBy(c => Math.Abs(c.Location.Latitude - latitude.Value) +
                                  Math.Abs(c.Location.Longitude - longitude.Value))
                    .ThenBy(c => c.Name),
                "name-desc" => query.OrderByDescending(c => c.Name),
                _ => query.OrderBy(c => c.Name)
            };

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.Practitioners)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            return new PagedResult<Clinic>(items, totalItems, page, pageSize);
        }
    }
}
