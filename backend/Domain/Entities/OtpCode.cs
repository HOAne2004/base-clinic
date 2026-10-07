using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Mã OTP cho 2FA / xác thực SĐT / reset password.
    /// Ban đầu fake — sinh mã và lưu DB. Sau này tích hợp SMS provider thật.
    /// </summary>
    public class OtpCode : Entity
    {
        public Guid AccountId { get; private set; }
        public Account Account { get; private set; } = null!;

        public string Code { get; private set; } = string.Empty;
        public OtpPurpose Purpose { get; private set; }
        public OtpChannel Channel { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? UsedAt { get; private set; }

        public int Attempts { get; private set; }
        public int MaxAttempts { get; private set; } = 5;

        /// <summary>OTP còn hiệu lực không (chưa dùng, chưa hết hạn, chưa vượt attempts).</summary>
        public bool IsValid =>
            UsedAt == null
            && ExpiresAt > DateTimeOffset.UtcNow
            && Attempts < MaxAttempts;

        private OtpCode() { } // EF Core

        /// <summary>
        /// Tạo OTP mới. Mã 6 chữ số, hết hạn sau `expiryMinutes` phút.
        /// </summary>
        public static OtpCode Create(
            Guid accountId,
            OtpPurpose purpose,
            OtpChannel channel,
            int expiryMinutes = 5)
        {
            return new OtpCode
            {
                AccountId = accountId,
                Code = GenerateCode(),
                Purpose = purpose,
                Channel = channel,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
            };
        }

        private static string GenerateCode()
            => Random.Shared.Next(100000, 999999).ToString();

        /// <summary>Đánh dấu OTP đã dùng — không dùng lại được.</summary>
        public void MarkAsUsed() => UsedAt = DateTimeOffset.UtcNow;

        /// <summary>Tăng số lần nhập sai. Dùng để chống brute-force.</summary>
        public void IncrementAttempts() => Attempts++;
    }
}
