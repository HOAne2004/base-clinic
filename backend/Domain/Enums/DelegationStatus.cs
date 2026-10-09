namespace BaseClinic.Domain.Enums
{
    /// <summary>
    /// Trạng thái của một ủy quyền theo dõi hồ sơ.
    /// </summary>
    public enum DelegationStatus
    {
        /// <summary>Đang chờ approve (khi C gửi request theo dõi B).</summary>
        Pending = 1,

        /// <summary>Đã được chấp nhận, đang có hiệu lực.</summary>
        Accepted = 2,

        /// <summary>Đã bị từ chối (khi A/B reject request).</summary>
        Rejected = 3,

        /// <summary>Đã bị thu hồi (sau khi đã Accepted).</summary>
        Revoked = 4
    }
}