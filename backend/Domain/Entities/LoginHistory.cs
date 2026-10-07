using BaseClinic.Domain.Common;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Lịch sử đăng nhập — phục vụ security audit và UX "thiết bị của tôi".
    /// Append-only: chỉ thêm, không sửa, không xóa.
    /// </summary>
    public class LoginHistory : Entity
    {
        public Guid AccountId { get; private set; }
        public Account Account { get; private set; } = null!;

        public DateTimeOffset LoginAt { get; private set; }
        public string? IpAddress { get; private set; }
        public string? DeviceInfo { get; private set; }

        public bool IsSuccess { get; private set; }
        public string? FailureReason { get; private set; }

        private LoginHistory() { } // EF Core

        private LoginHistory(
            Guid accountId,
            DateTimeOffset loginAt,
            string? ipAddress,
            string? deviceInfo,
            bool isSuccess,
            string? failureReason)
        {
            AccountId = accountId;
            LoginAt = loginAt;
            IpAddress = ipAddress;
            DeviceInfo = deviceInfo;
            IsSuccess = isSuccess;
            FailureReason = failureReason;
        }

        /// <summary>Factory method cho đăng nhập thành công.</summary>
        public static LoginHistory Success(
            Guid accountId,
            DateTimeOffset loginAt,
            string? ipAddress,
            string? deviceInfo)
            => new(accountId, loginAt, ipAddress, deviceInfo, true, null);

        /// <summary>Factory method cho đăng nhập thất bại.</summary>
        public static LoginHistory Failed(
            Guid accountId,
            DateTimeOffset loginAt,
            string? ipAddress,
            string? deviceInfo,
            string? reason)
            => new(accountId, loginAt, ipAddress, deviceInfo, false, reason);
    }
}
