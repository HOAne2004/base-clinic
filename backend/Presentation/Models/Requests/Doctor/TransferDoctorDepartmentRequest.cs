using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Doctor
{
    public class TransferDoctorDepartmentRequest
    {
        [Required(ErrorMessage = "NewDepartmentId không được để trống.")]
        public Guid NewDepartmentId { get; set; }
    }
}