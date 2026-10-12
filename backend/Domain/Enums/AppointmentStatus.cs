namespace BaseClinic.Domain.Enums
{
    /// <summary>
    /// Trạng thái của lịch hẹn.
    /// 
    /// LUỒNG CHUYỂN TRẠNG THÁI:
    /// 
    ///     Pending ──confirm──> Confirmed
    ///        │                    │
    ///        │                    ├──check-in──> CheckedIn ──complete──> Completed
    ///        │                    │
    ///        │                    ├──no-show──> NoShow
    ///        │                    │
    ///        └──cancel────────────┴──cancel──> Cancelled
    /// </summary>
    public enum AppointmentStatus
    {
        /// <summary>
        /// Chờ xác nhận — user vừa đặt online, chờ lễ tân/bác sĩ xác nhận.
        /// Nếu đặt trực tiếp tại quầy hoặc qua receptionist → có thể bỏ qua Pending.
        /// </summary>
        Pending = 0,

        /// <summary>Đã xác nhận — sẵn sàng cho check-in.</summary>
        Confirmed = 1,

        /// <summary>Bệnh nhân đã đến và check-in tại phòng khám.</summary>
        CheckedIn = 2,

        /// <summary>Đã hoàn tất — bác sĩ đã khám xong.</summary>
        Completed = 3,

        /// <summary>Đã hủy — bởi bệnh nhân hoặc bác sĩ.</summary>
        Cancelled = 4,

        /// <summary>Bệnh nhân không đến và không thông báo.</summary>
        NoShow = 5
    }
}