using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class OtpCodeRepository : Repository<OtpCode>, IOtpCodeRepository
    {
        public OtpCodeRepository(ClinicDbContext context) : base(context)
        {
        }
        public async Task<OtpCode?> GetLatestValidAsync(
            Guid accountId,
            OtpPurpose purpose,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            return await _dbSet
                .Where(o => o.AccountId == accountId
                         && o.Purpose == purpose
                         && o.UsedAt == null
                         && o.ExpiresAt > now
                         && o.Attempts < o.MaxAttempts)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task InvalidateAllPendingAsync(
            Guid accountId,
            OtpPurpose purpose,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            var pendingOtps = await _dbSet
                .Where(o => o.AccountId == accountId
                         && o.Purpose == purpose
                         && o.UsedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var otp in pendingOtps)
                otp.MarkAsUsed();
        }

        public async Task<DateTimeOffset?> GetLatestCreatedTimeAsync(
            Guid accountId,
            OtpPurpose purpose,
            CancellationToken cancellationToken = default)
        {
            var latest = await _dbSet
                .Where(o => o.AccountId == accountId && o.Purpose == purpose)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => (DateTimeOffset?)o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            return latest;
        }
    }
}
