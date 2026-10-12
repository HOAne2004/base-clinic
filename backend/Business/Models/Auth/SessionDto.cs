namespace BaseClinic.Business.Models.Auth
{
    /// <summary>
    /// Thông tin 1 phiên đăng nhập (thiết bị) đang active.
    /// Dùng cho trang "Quản lý thiết bị" của user.
    /// </summary>
    public record SessionDto(
        Guid SessionId,
        string? DeviceInfo,
        string? IpAddress,
        DateTimeOffset CreatedAt,
        DateTimeOffset ExpiresAt,
        bool IsCurrent,
        bool IsTrusted
    );
}