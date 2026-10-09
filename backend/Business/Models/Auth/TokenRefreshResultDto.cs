namespace BaseClinic.Business.Models.Auth
{
    public record TokenRefreshResultDto(string AccessToken, string RefreshToken);
}
