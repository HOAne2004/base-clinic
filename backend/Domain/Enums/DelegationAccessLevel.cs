namespace BaseClinic.Domain.Enums
{
    /// <summary>
    /// Mức độ truy cập của một ủy quyền theo dõi hồ sơ.
    /// </summary>
    public enum DelegationAccessLevel
    {
        /// <summary>Chỉ xem được lịch sử do chính Observer tạo.</summary>
        Limited = 1,

        /// <summary>Xem được mọi lịch sử của Patient (kể cả do người khác tạo).</summary>
        Full = 2
    }
}