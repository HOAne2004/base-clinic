using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Lấy RefreshToken theo hash, kèm Account + danh sách RefreshTokens của Account.
        /// Dùng cho RefreshToken flow — cần Account.RefreshTokens để rotate.
        /// </summary>
        Task<RefreshToken?> GetByTokenHashWithAccountAsync(
            string tokenHash,
            CancellationToken cancellationToken = default);

        /// <summary>Lấy danh sách session đang active của Account.</summary>
        Task<List<RefreshToken>> GetActiveSessionsByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default);
    }
}