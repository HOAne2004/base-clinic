using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenHashWithAccountAsync(
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rt => rt.Account)
                    .ThenInclude(a => a.RefreshTokens)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<List<RefreshToken>> GetActiveSessionsByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            return await _dbSet
                .Where(rt => rt.AccountId == accountId
                          && rt.RevokedAt == null
                          && rt.ExpiresAt > now)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}