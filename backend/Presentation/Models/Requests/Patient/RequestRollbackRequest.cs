using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Patient
{
    public class RequestRollbackRequest
    {
        [Required(ErrorMessage = "Lý do không được để trống.")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}