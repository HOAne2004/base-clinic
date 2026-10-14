using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Input cập nhật ca làm việc.
    /// Chỉ sửa được nếu CHƯA có bệnh nhân đặt slot.
    /// </summary>
    public class UpdateScheduleDto
    {
        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Range(5, 120)]
        public int SlotDurationMinutes { get; set; }

        [Range(1, 50)]
        public int MaxPatientPerSlot { get; set; }

        public bool OnlineBookingEnabled { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }
    }
}