using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Lịch làm việc của 1 bác sĩ trong 1 ngày cụ thể.
    /// 
    /// KHÁC BIỆT so với DoctorScheduleItem:
    /// - DoctorSchedule = "ca làm việc" (VD: 8:00-12:00).
    /// - DoctorScheduleItem = "slot cụ thể" (VD: 8:00-8:15).
    /// 
    /// Khi tạo schedule → tự động sinh các items (slots) từ StartTime/EndTime/SlotDuration.
    /// </summary>
    public class DoctorSchedule : FullAuditedEntity
    {
        // ==================== HẰNG SỐ ====================
        private const int MinSlotDuration = 5;
        private const int MaxSlotDuration = 120;
        private const int MaxBookableDaysAhead = 90;

        // ==================== LIÊN KẾT ====================
        public Guid DoctorId { get; private set; }
        public Doctor Doctor { get; private set; } = null!;

        // ==================== THỜI GIAN ====================
        /// <summary>Ngày làm việc (chỉ ngày, không giờ).</summary>
        public DateOnly WorkDate { get; private set; }

        /// <summary>Giờ bắt đầu ca.</summary>
        public TimeOnly StartTime { get; private set; }

        /// <summary>Giờ kết thúc ca.</summary>
        public TimeOnly EndTime { get; private set; }

        // ==================== CẤU HÌNH SLOT ====================
        /// <summary>Thời lượng mỗi slot (phút). VD: 15, 20, 30.</summary>
        public int SlotDurationMinutes { get; private set; }

        /// <summary>Số bệnh nhân tối đa mỗi slot.</summary>
        public int MaxPatientPerSlot { get; private set; }

        // ==================== ĐẶT LỊCH ONLINE ====================
        /// <summary>Ca này có mở cho bệnh nhân đặt online không.</summary>
        public bool OnlineBookingEnabled { get; private set; } = true;

        // ==================== GHI CHÚ ====================
        public string? Note { get; private set; }

        // ==================== TRẠNG THÁI ====================
        public DoctorScheduleStatus Status { get; private set; }

        // ==================== HỦY CA ====================
        public DateTimeOffset? CancelledAt { get; private set; }
        public Guid? CancelledBy { get; private set; }
        public string? CancelReason { get; private set; }

        // ==================== NAVIGATION ====================
        private readonly List<DoctorScheduleItem> _items = new();
        public IReadOnlyCollection<DoctorScheduleItem> Items => _items.AsReadOnly();

        // ==================== CONSTRUCTOR ====================
        private DoctorSchedule() { } // EF Core

        public DoctorSchedule(
            Guid doctorId,
            DateOnly workDate,
            TimeOnly startTime,
            TimeOnly endTime,
            int slotDurationMinutes,
            int maxPatientPerSlot,
            bool onlineBookingEnabled = true,
            string? note = null)
        {
            if (doctorId == Guid.Empty)
                throw new DomainException("DoctorId không hợp lệ.");

            DoctorId = doctorId;
            ValidateAndSetScheduleTime(workDate, startTime, endTime, slotDurationMinutes);
            ValidateAndSetCapacity(maxPatientPerSlot);

            OnlineBookingEnabled = onlineBookingEnabled;
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            Status = DoctorScheduleStatus.Available;
        }

        // ==================== UPDATE ====================
        public void UpdateSchedule(
            TimeOnly startTime,
            TimeOnly endTime,
            int slotDurationMinutes,
            int maxPatientPerSlot,
            bool onlineBookingEnabled,
            string? note = null)
        {
            if (Status is DoctorScheduleStatus.Completed or DoctorScheduleStatus.Cancelled)
                throw new DomainException("Không thể sửa ca đã kết thúc hoặc đã hủy.");

            if (_items.Any(i => i.BookedCount > 0))
                throw new DomainException(
                    "Không thể sửa ca đã có bệnh nhân đặt slot. " +
                    "Vui lòng hủy các appointment trước.");

            ValidateAndSetScheduleTime(WorkDate, startTime, endTime, slotDurationMinutes);
            ValidateAndSetCapacity(maxPatientPerSlot);

            OnlineBookingEnabled = onlineBookingEnabled;
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        }

        /// <summary>
        /// Cập nhật ghi chú.
        /// </summary>
        public void UpdateNote(string? note)
        {
            if (Status is DoctorScheduleStatus.Completed or DoctorScheduleStatus.Cancelled)
                throw new DomainException("Không thể sửa ghi chú của ca đã kết thúc.");

            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        }

        /// <summary>
        /// Bật/tắt đặt online cho ca này.
        /// </summary>
        public void SetOnlineBookingEnabled(bool enabled)
        {
            if (Status is DoctorScheduleStatus.Completed or DoctorScheduleStatus.Cancelled)
                throw new DomainException("Không thể đổi trạng thái ca đã kết thúc.");

            OnlineBookingEnabled = enabled;
        }

        // ==================== VALIDATION ====================
        private void ValidateAndSetScheduleTime(
            DateOnly workDate,
            TimeOnly startTime,
            TimeOnly endTime,
            int slotDurationMinutes)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (workDate < today)
                throw new DomainException("Ngày làm việc không thể ở quá khứ.");

            if (workDate > today.AddDays(MaxBookableDaysAhead))
                throw new DomainException(
                    $"Chỉ có thể tạo lịch trong vòng {MaxBookableDaysAhead} ngày tới.");

            if (startTime >= endTime)
                throw new DomainException(
                    "Thời gian bắt đầu phải trước thời gian kết thúc.");

            if (slotDurationMinutes < MinSlotDuration || slotDurationMinutes > MaxSlotDuration)
                throw new DomainException(
                    $"Thời lượng slot phải từ {MinSlotDuration} đến {MaxSlotDuration} phút.");

            // Tổng thời gian phải chia hết cho slot duration
            var totalMinutes = (endTime.ToTimeSpan() - startTime.ToTimeSpan()).TotalMinutes;
            if (totalMinutes % slotDurationMinutes != 0)
                throw new DomainException(
                    "Tổng thời gian ca phải chia hết cho thời lượng slot.");

            WorkDate = workDate;
            StartTime = startTime;
            EndTime = endTime;
            SlotDurationMinutes = slotDurationMinutes;
        }

        private void ValidateAndSetCapacity(int maxPatientPerSlot)
        {
            if (maxPatientPerSlot < 1)
                throw new DomainException("Số bệnh nhân tối đa mỗi slot phải >= 1.");

            if (maxPatientPerSlot > 50)
                throw new DomainException("Số bệnh nhân tối đa mỗi slot không được vượt quá 50.");

            MaxPatientPerSlot = maxPatientPerSlot;
        }

        // ==================== ITEMS (Slots) ====================
        /// <summary>
        /// Thêm item (slot) vào schedule.
        /// Được gọi khi tạo schedule — sinh các slot tự động.
        /// </summary>
        internal void AddItem(DoctorScheduleItem item)
        {
            if (item == null)
                throw new DomainException("Item không được null.");

            _items.Add(item);
        }

        /// <summary>
        /// Xóa toàn bộ items (khi update schedule).
        /// </summary>
        internal void ClearItems()
        {
            if (_items.Any(i => i.BookedCount > 0))
                throw new DomainException("Không thể xóa các slot đã có bệnh nhân đặt.");

            _items.Clear();
        }

        // ==================== TRẠNG THÁI CA ====================
        public void Cancel(Guid cancelledByAccountId, string reason)
        {
            if (Status == DoctorScheduleStatus.Completed)
                throw new DomainException("Không thể hủy ca đã hoàn thành.");

            if (Status == DoctorScheduleStatus.Cancelled)
                throw new DomainException("Ca này đã được hủy trước đó.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Lý do hủy không được để trống.");

            Status = DoctorScheduleStatus.Cancelled;
            CancelledAt = DateTimeOffset.UtcNow;
            CancelledBy = cancelledByAccountId;
            CancelReason = reason.Trim();
        }

        public void Complete()
        {
            if (Status == DoctorScheduleStatus.Cancelled)
                throw new DomainException("Không thể hoàn thành ca đã hủy.");

            if (Status == DoctorScheduleStatus.Completed)
                throw new DomainException("Ca này đã hoàn thành trước đó.");

            Status = DoctorScheduleStatus.Completed;
        }
    }
}