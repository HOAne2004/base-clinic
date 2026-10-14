using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Department
{
    public class SetDepartmentOnlineBookingRequest
    {
        [Required(ErrorMessage = "Enabled không được để trống.")]
        public bool Enabled { get; set; }
    }
}