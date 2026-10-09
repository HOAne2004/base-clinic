using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không vượt quá 20 ký tự.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6-100 ký tự.")]
        public string Password { get; set; } = string.Empty;
    }
}