namespace BaseClinic.Domain.Enums
{
    /// <summary>
    /// Trạng thái yêu cầu hủy ca làm việc.
    /// 
    /// LUỒNG:
    ///     Pending ──approve──> Approved  (Admin duyệt → hủy ca)
    ///        │
    ///        ├──reject──> Rejected      (Admin từ chối)
    ///        │
    ///        └──cancel──> Cancelled     (Requester tự hủy)
    /// </summary>
    public enum ScheduleCancellationRequestStatus
    {
        /// <summary>Đang chờ Admin duyệt.</summary>
        Pending = 1,

        /// <summary>Đã duyệt — ca đã được hủy.</summary>
        Approved = 2,

        /// <summary>Bị từ chối bởi Admin.</summary>
        Rejected = 3,

        /// <summary>Requester tự hủy trước khi duyệt.</summary>
        Cancelled = 4
    }
}