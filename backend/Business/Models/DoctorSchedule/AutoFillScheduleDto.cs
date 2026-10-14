using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Input auto-fill nhiều ca cho 1 bác sĩ.
    /// Hỗ trợ 2 ca mặc định: sáng + chiều (chiều optional).
    /// </summary>
    public class AutoFillScheduleDto
    {
        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Các thứ trong tuần cần tạo.
        /// VD: [Monday, Tuesday, Wednesday, Thursday, Friday].
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất 1 ngày trong tuần.")]
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        // ==================== CA SÁNG ====================
        [Required]
        public TimeOnly MorningStart { get; set; }

        [Required]
        public TimeOnly MorningEnd { get; set; }

        // ==================== CA CHIỀU ====================
        public TimeOnly? AfternoonStart { get; set; }

        public TimeOnly? AfternoonEnd { get; set; }

        // ==================== CẤU HÌNH SLOT ====================
        [Range(5, 120)]
        public int SlotDurationMinutes { get; set; } = 20;

        [Range(1, 50)]
        public int MaxPatientPerSlot { get; set; } = 1;

        public bool OnlineBookingEnabled { get; set; } = true;

        [StringLength(500)]
        public string? Note { get; set; }

        /// <summary>
        /// Skip các ngày đã có schedule. Default = true.
        /// </summary>
        public bool SkipExistingDays { get; set; } = true;
    }
}