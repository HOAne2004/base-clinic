using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IOtpCodeRepository : IRepository<OtpCode>
    {
        /// <summary>
        /// Lấy OTP mới nhất còn hiệu lực (chưa dùng, chưa hết hạn, chưa vượt attempts).
        /// </summary>
        Task<OtpCode?> GetLatestValidAsync(
            Guid accountId,
            OtpPurpose purpose,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Vô hiệu hóa toàn bộ OTP đang pending của 1 Account + Purpose.
        /// Dùng khi tạo OTP mới — tránh nhiều OTP cùng tồn tại.
        /// </summary>
        Task InvalidateAllPendingAsync(
            Guid accountId,
            OtpPurpose purpose,
            CancellationToken cancellationToken = default);
    }
}
