using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Auth
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "AccountId không được để trống.")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Mã OTP không được để trống.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Mã OTP không hợp lệ.")]
        public string OtpCode { get; set; } = string.Empty;
    }
}