namespace BaseClinic.Business.Models
{
    public record AuthResultDto(
        string AccessToken,
        AccountPublicInfoDto User
    );
}
