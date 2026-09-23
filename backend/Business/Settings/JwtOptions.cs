namespace BaseClinic.Business.Settings
{
    public class JwtOptions
    {
        public const string SectionName = "JwtSettings"; // Dùng để map trong Program.cs

        public string Key { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; init; } = 15;
        public int RefreshTokenExpirationDays { get; init; } = 7;
    }
}
