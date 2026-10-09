namespace BaseClinic.Domain.Constants
{
    /// <summary>
    /// Các hằng số dùng chung toàn hệ thống.
    /// </summary>
    public static class SystemConstants
    {
        /// <summary>
        /// Guid đại diện cho "hệ thống" khi thực hiện hành động tự động.
        /// Dùng cho audit fields (GrantedBy, AssignedBy, CreatedBy...) khi không có user cụ thể.
        /// </summary>
        public static readonly Guid SystemUserId = Guid.Empty;

        /// <summary>
        /// Tên hiển thị cho system user.
        /// </summary>
        public const string SystemUserName = "SYSTEM";
    }
}