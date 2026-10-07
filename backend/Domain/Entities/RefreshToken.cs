using BaseClinic.Domain.Common;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Refresh token cho multi-device login.
    /// Mỗi login thành công → tạo 1 token. Max 2 thiết bị active.
    /// Không kế thừa AuditableEntity vì đã có CreatedAt, RevokedAt riêng.
    /// </summary>
    public class RefreshToken : Entity
    {
        public Guid AccountId { get; private set; }
        public Account Account { get; private set; } = null!;

        /// <summary>Hash của refresh token (KHÔNG lưu plain text).</summary>
        public string TokenHash { get; private set; } = string.Empty;

        /// <summary>Thông tin thiết bị để user phân biệt khi revoke. VD: "Chrome on Windows".</summary>
        public string? DeviceInfo { get; private set; }

        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? RevokedAt { get; private set; }
        public string? RevokeReason { get; private set; }

        /// <summary>Thiết bị tin cậy — không cần OTP lại.</summary>
        public bool IsTrusted { get; private set; }

        /// <summary>Token còn hiệu lực không.</summary>
        public bool IsActive => RevokedAt == null && ExpiresAt > DateTimeOffset.UtcNow;

        private RefreshToken() { } // EF Core

        public RefreshToken(
            Guid accountId,
            string tokenHash,
            DateTimeOffset expiresAt,
            string? deviceInfo = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new DomainException("Token hash không được để trống.");
            if (expiresAt <= DateTimeOffset.UtcNow)
                throw new DomainException("Thời gian hết hạn phải ở tương lai.");

            AccountId = accountId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
            CreatedAt = DateTimeOffset.UtcNow;
            DeviceInfo = deviceInfo;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }

        /// <summary>Thu hồi token. Idempotent — gọi nhiều lần không lỗi.</summary>
        public void Revoke(string reason)
        {
            if (RevokedAt != null)
                return;

            RevokedAt = DateTimeOffset.UtcNow;
            RevokeReason = reason;
        }

        /// <summary>Đánh dấu thiết bị tin cậy (không cần OTP lần sau).</summary>
        public void MarkAsTrusted() => IsTrusted = true;
    }
}
