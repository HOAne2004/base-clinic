using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Auth
{
    public class ResendOtpRequest
    {
        [Required(ErrorMessage = "AccountId không được để trống.")]
        public Guid AccountId { get; set; }
    }
}