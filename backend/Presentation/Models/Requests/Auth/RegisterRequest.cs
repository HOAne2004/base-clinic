using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không vượt quá 20 ký tự.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
        public string Password { get; set; } = string.Empty;
    }
}
