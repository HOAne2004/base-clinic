namespace BaseClinic.Business.Models.Auth
{
    /// <summary>
    /// Kết quả sau khi đăng ký thành công.
    /// CHỈ chứa thông tin để FE chuyển sang bước verify OTP.
    /// KHÔNG chứa AccessToken/RefreshToken — vì account chưa Active.
    /// </summary>
    public record RegisterResultDto(
        Guid AccountId,
        string PhoneNumber,
        DateTimeOffset OtpExpiresAt,
        bool RequiresOtpVerification);
}
