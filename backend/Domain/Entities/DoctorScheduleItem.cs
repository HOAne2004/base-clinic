using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Một slot cụ thể trong ca làm việc của bác sĩ.
    /// 
    /// VD: Ca 8:00-12:00, SlotDuration=15 → sinh 16 items:
    ///     8:00-8:15, 8:15-8:30, ..., 11:45-12:00.
    /// 
    /// Appointment sẽ tham chiếu tới item để giữ chỗ.
    /// </summary>
    public class DoctorScheduleItem : Entity
    {
        // ==================== LIÊN KẾT ====================
        public Guid DoctorScheduleId { get; private set; }
        public DoctorSchedule DoctorSchedule { get; private set; } = null!;

        // ==================== THỜI GIAN ====================
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }

        // ==================== SỨC CHỨA ====================
        /// <summary>Sức chứa tối đa của slot này.</summary>
        public int Capacity { get; private set; }

        /// <summary>Số lượt đã được đặt (đếm sẵn).</summary>
        public int BookedCount { get; private set; }

        // ==================== TRẠNG THÁI ====================
        public DoctorScheduleItemStatus Status { get; private set; }

        // ==================== CONCURRENCY ====================
        /// <summary>
        /// RowVersion — chống race condition khi 2 request cùng book slot cuối.
        /// EF Core tự tăng khi update → throw DbUpdateConcurrencyException nếu conflict.
        /// </summary>
        public byte[] RowVersion { get; set; } = null!;

        // ==================== CONSTRUCTOR ====================
        private DoctorScheduleItem() { } // EF Core

        internal DoctorScheduleItem(
            Guid doctorScheduleId,
            TimeOnly startTime,
            TimeOnly endTime,
            int capacity)
        {
            if (doctorScheduleId == Guid.Empty)
                throw new DomainException("DoctorScheduleId không hợp lệ.");

            if (startTime >= endTime)
                throw new DomainException("StartTime phải trước EndTime.");

            if (capacity < 1)
                throw new DomainException("Capacity phải >= 1.");

            DoctorScheduleId = doctorScheduleId;
            StartTime = startTime;
            EndTime = endTime;
            Capacity = capacity;
            BookedCount = 0;
            Status = DoctorScheduleItemStatus.Available;
        }

        // ==================== BUSINESS METHODS ====================

        /// <summary>
        /// Slot còn chỗ để đặt không.
        /// </summary>
        public bool IsAvailableForBooking()
            => Status == DoctorScheduleItemStatus.Available
            && BookedCount < Capacity;

        /// <summary>
        /// Số chỗ còn lại.
        /// </summary>
        public int AvailableSlots => Math.Max(0, Capacity - BookedCount);

        /// <summary>
        /// Đặt 1 chỗ. Throw nếu không còn chỗ hoặc slot không available.
        /// </summary>
        public void BookOne()
        {
            if (Status != DoctorScheduleItemStatus.Available)
                throw new DomainException(
                    $"Slot hiện không khả dụng (status: {Status}).");

            if (BookedCount >= Capacity)
                throw new DomainException("Slot đã hết chỗ.");

            BookedCount++;

            // Đánh dấu Full nếu đã đầy
            if (BookedCount >= Capacity)
                Status = DoctorScheduleItemStatus.Full;
        }

        /// <summary>
        /// Giải phóng 1 chỗ (khi appointment bị hủy).
        /// </summary>
        public void ReleaseOne()
        {
            if (BookedCount <= 0)
                return;   // Idempotent — không throw

            BookedCount--;

            // Chuyển từ Full về Available nếu còn chỗ
            if (Status == DoctorScheduleItemStatus.Full && BookedCount < Capacity)
                Status = DoctorScheduleItemStatus.Available;
        }

        /// <summary>
        /// Khóa slot thủ công — không cho đặt.
        /// Throw nếu đã có người đặt.
        /// </summary>
        public void Block()
        {
            if (BookedCount > 0)
                throw new DomainException(
                    "Không thể khóa slot đã có bệnh nhân đặt.");

            if (Status == DoctorScheduleItemStatus.Blocked)
                return;   // Idempotent

            Status = DoctorScheduleItemStatus.Blocked;
        }

        /// <summary>
        /// Mở khóa slot.
        /// </summary>
        public void Unblock()
        {
            if (Status != DoctorScheduleItemStatus.Blocked)
                return;   // Idempotent

            Status = BookedCount >= Capacity
                ? DoctorScheduleItemStatus.Full
                : DoctorScheduleItemStatus.Available;
        }

        /// <summary>
        /// Đóng slot (khi đã qua thời gian).
        /// </summary>
        public void Close()
        {
            if (Status == DoctorScheduleItemStatus.Closed)
                return;   // Idempotent

            Status = DoctorScheduleItemStatus.Closed;
        }
    }
}