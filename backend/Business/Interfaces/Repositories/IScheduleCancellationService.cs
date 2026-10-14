namespace BaseClinic.Business.Interfaces.Services
{
    /// <summary>
    /// Kết quả sau khi hủy ca.
    /// </summary>
    public record CancelScheduleResultDto(
        int CancelledAppointments,
        int ReleasedSlots,
        List<Guid> CancelledAppointmentIds
    );

    /// <summary>
    /// Service xử lý hủy ca — bao gồm cascade appointments + release slots + SMS.
    /// Được dùng bởi cả CancelScheduleCommand và ApproveCancelScheduleCommand.
    /// </summary>
    public interface IScheduleCancellationService
    {
        /// <summary>
        /// Hủy ca làm việc:
        /// 1. Check không có appointment đã CheckedIn/Completed.
        /// 2. Hủy tất cả appointment chưa CheckedIn.
        /// 3. Release slots.
        /// 4. Đổi Schedule.Status = Cancelled.
        /// 5. Gửi SMS cho bệnh nhân.
        /// 
        /// LƯU Ý: KHÔNG gọi SaveChanges — Handler gọi.
        /// </summary>
        Task<CancelScheduleResultDto> CancelAsync(
            Guid scheduleId,
            Guid cancelledByAccountId,
            string reason,
            CancellationToken cancellationToken = default);
    }
}