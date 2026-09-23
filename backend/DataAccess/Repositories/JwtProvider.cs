using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.Business.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BaseClinic.DataAccess.Repositories
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;

        public JwtProvider(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateAccessToken(TokenUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("SecurityStamp", user.SecurityStamp.ToString()),
            new Claim("SessionId", user.SessionId.ToString())
        };

            // Giữ lại việc nhúng Role nếu cần thiết cho UI
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // CỐT LÕI CỦA RBAC ĐỘNG: Nhúng toàn bộ Permission vào Token
            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
                SigningCredentials = creds,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public DateTime GetRefreshTokenExpiry() => DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
        public DateTime GetAccessTokenExpiry() => DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        public string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
