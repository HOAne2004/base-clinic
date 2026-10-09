using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        /// <summary>
        /// Lấy Account theo SĐT, kèm RefreshTokens (cần cho multi-device login).
        /// </summary>
        Task<Account?> GetByPhoneNumberAsync(
            string phoneNumber,
            CancellationToken cancellationToken = default);

        /// <summary>Lấy danh sách mã Role của Account (VD: ["ADMIN", "DOCTOR"]).</summary>
        Task<List<string>> GetRoleCodesByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default);

        /// <summary>Lấy danh sách mã Permission của Account (union từ các Role).</summary>
        Task<List<string>> GetPermissionCodesByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check SĐT đã tồn tại chưa. Dùng AnyAsync — không load entity.
        /// Dùng cho Register.
        /// </summary>
        Task<bool> ExistsByPhoneAsync(string phoneNumber, CancellationToken ct = default);

        /// <summary>
        /// Lấy Account theo Id, kèm RefreshTokens.
        /// Dùng cho VerifyOtp / RefreshToken — cần thiết để AddRefreshToken đúng.
        /// </summary>
        Task<Account?> GetByIdWithRefreshTokensAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}