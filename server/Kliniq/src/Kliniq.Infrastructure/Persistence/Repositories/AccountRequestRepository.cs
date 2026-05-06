using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

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
    }
}
