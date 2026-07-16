using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Common.Models;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Persistence.Repositories
{
    public class AccountRequestRepository : IAccountRequestRepository
    {
        private readonly AppDbContext _context;

        public AccountRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AccountRequest accountRequest, CancellationToken cancellationToken)
            => await _context.AccountRequests.AddAsync(accountRequest, cancellationToken);

        public async Task<AccountRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.AccountRequests.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<bool> ExistsPendingEmailAsync(string email, CancellationToken cancellationToken)
            => await _context.AccountRequests.AnyAsync(a => a.Email == email && a.Status == AccountRequestStatus.Pending, cancellationToken);

        public async Task<AccountRequest?> GetApprovedByEmailAsync(string email, CancellationToken cancellationToken)
            => await _context.AccountRequests.FirstOrDefaultAsync(a => a.Email == email && a.Status == AccountRequestStatus.Approved, cancellationToken);

        public async Task<AccountRequest?> GetByInvitationTokenAsync(string invitationToken, CancellationToken cancellationToken)
            => await _context.AccountRequests.FirstOrDefaultAsync(a => a.InvitationToken == invitationToken 
                                                                  && a.Status == AccountRequestStatus.Approved 
                                                                  && !a.IsInvitationUsed && a.InvitationExpiresAt > DateTime.UtcNow, cancellationToken);

        public async Task<PagedResult<AccountRequest>> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            string? status,
            CancellationToken cancellationToken)
        {
            (page, pageSize) = Pagination.Normalize(page, pageSize);
            var query = _context.AccountRequests.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(request =>
                    request.Email.Contains(term) ||
                    request.LicenseNumber.Contains(term) ||
                    request.Name.FirstName.Contains(term) ||
                    request.Name.LastName.Contains(term) ||
                    request.ClinicName.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AccountRequestStatus>(status, true, out var parsedStatus))
                query = query.Where(request => request.Status == parsedStatus);

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(request => request.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AccountRequest>(items, totalItems, page, pageSize);
        }

    }
}
