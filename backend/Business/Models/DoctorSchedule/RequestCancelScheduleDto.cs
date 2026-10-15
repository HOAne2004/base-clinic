using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    public class RequestCancelScheduleDto
    {
        [Required(ErrorMessage = "Lý do hủy không được để trống.")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}