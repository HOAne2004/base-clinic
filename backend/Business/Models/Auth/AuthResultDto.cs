using BaseClinic.Business.Models.Account;

namespace BaseClinic.Business.Models.Auth
{
    public record AuthResultDto(
        string AccessToken,
        string RefreshToken,
        AccountPublicInfoDto User,
        bool RequiresPasswordChange = false
    );
}
