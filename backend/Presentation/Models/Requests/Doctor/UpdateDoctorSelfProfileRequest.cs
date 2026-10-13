using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Doctor
{
    public class UpdateDoctorSelfProfileRequest
    {
        [StringLength(500)]
        public string? Avatar { get; set; }

        [StringLength(2000)]
        public string? Biography { get; set; }

        [StringLength(100)]
        public string? Languages { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public bool IsAvailableForOnline { get; set; } = true;
    }
}