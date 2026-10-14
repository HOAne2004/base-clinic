using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Input hủy ca làm việc.
    /// </summary>
    public class CancelScheduleDto
    {
        [Required(ErrorMessage = "Lý do hủy không được để trống.")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}