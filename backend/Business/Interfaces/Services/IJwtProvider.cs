using BaseClinic.Business.Models.Auth;
using System.Security.Claims;

namespace BaseClinic.Business.Interfaces.Services
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(TokenUser user);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiry();
        DateTime GetAccessTokenExpiry();
        string HashToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
