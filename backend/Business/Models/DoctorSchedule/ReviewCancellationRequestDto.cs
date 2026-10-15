using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    public class ReviewCancellationRequestDto
    {
        [StringLength(500)]
        public string? ReviewNote { get; set; }
    }
}