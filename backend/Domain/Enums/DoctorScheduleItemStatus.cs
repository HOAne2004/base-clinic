namespace BaseClinic.Domain.Enums
{
    /// <summary>
    /// Trạng thái của một slot cụ thể trong ca làm việc.
    /// </summary>
    public enum DoctorScheduleItemStatus
    {
        /// <summary>Còn chỗ, bệnh nhân có thể đặt.</summary>
        Available = 1,

        /// <summary>Đã đầy — bookedCount >= capacity.</summary>
        Full = 2,

        /// <summary>Bị khóa thủ công bởi nhân viên (không cho đặt).</summary>
        Blocked = 3,

        /// <summary>Slot đã qua thời gian (workDate + startTime < now).</summary>
        Closed = 4
    }
}