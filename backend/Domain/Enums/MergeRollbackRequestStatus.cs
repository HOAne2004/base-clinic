namespace BaseClinic.Domain.Enums
{
    /// <summary>
    /// Trạng thái của yêu cầu rollback merge.
    /// 
    /// LUỒNG:
    ///     Pending ──approve──> Approved
    ///        │
    ///        ├──reject──> Rejected
    ///        │
    ///        └──cancel──> Cancelled
    /// </summary>
    public enum MergeRollbackRequestStatus
    {
        /// <summary>Đang chờ Admin/SuperAdmin duyệt.</summary>
        Pending = 1,

        /// <summary>Đã duyệt — rollback đã được thực hiện.</summary>
        Approved = 2,

        /// <summary>Bị từ chối bởi Admin/SuperAdmin.</summary>
        Rejected = 3,

        /// <summary>Requester tự hủy trước khi duyệt.</summary>
        Cancelled = 4
    }
}