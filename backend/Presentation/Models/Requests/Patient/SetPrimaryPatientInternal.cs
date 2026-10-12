using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Patient
{
    public class MergePatientRequest
    {
        [Required(ErrorMessage = "SurvivorPatientId không được để trống.")]
        public Guid SurvivorPatientId { get; set; }

        [Required(ErrorMessage = "MergedPatientId không được để trống.")]
        public Guid MergedPatientId { get; set; }

        [Required(ErrorMessage = "Lý do merge không được để trống.")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        // Danh cho admin check thu cong
        public bool ConfirmNoIdentity { get; set; }
    }
}