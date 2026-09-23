using BaseClinic.Business.Models;

namespace BaseClinic.Business.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(TokenUser user);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiry();
        DateTime GetAccessTokenExpiry();
        string HashToken(string token);
    }
}
