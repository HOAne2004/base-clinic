using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token không được để trống.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}