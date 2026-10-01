namespace BaseClinic.Business.Models
{
    public record AuthResultDto(
        string AccessToken,
        string RefreshToken,
        AccountPublicInfoDto User
    );
}
