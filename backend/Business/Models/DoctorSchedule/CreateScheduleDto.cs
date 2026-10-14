using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Input tạo 1 ca làm việc đơn.
    /// </summary>
    public class CreateScheduleDto
    {
        [Required(ErrorMessage = "DoctorId không được để trống.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Ngày làm việc không được để trống.")]
        public DateOnly WorkDate { get; set; }

        [Required(ErrorMessage = "Giờ bắt đầu không được để trống.")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc không được để trống.")]
        public TimeOnly EndTime { get; set; }

        /// <summary>Thời lượng mỗi slot (phút). VD: 15, 20, 30.</summary>
        [Range(5, 120, ErrorMessage = "Thời lượng slot phải từ 5-120 phút.")]
        public int SlotDurationMinutes { get; set; } = 20;

        /// <summary>Số bệnh nhân tối đa mỗi slot.</summary>
        [Range(1, 50, ErrorMessage = "Số bệnh nhân mỗi slot phải từ 1-50.")]
        public int MaxPatientPerSlot { get; set; } = 1;

        public bool OnlineBookingEnabled { get; set; } = true;

        [StringLength(500)]
        public string? Note { get; set; }
    }
}