using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.DoctorSchedule
{
    public class RejectCancellationRequestDto
    {
        [Required(ErrorMessage = "Lý do từ chối không được để trống.")]
        [StringLength(500)]
        public string ReviewNote { get; set; } = string.Empty;
    }
}
