namespace BaseClinic.Business.Models.Auth
{
    /// <summary>
    /// Kết quả gửi lại OTP.
    /// </summary>
    public record ResendOtpResultDto(
        string PhoneNumber,
        DateTimeOffset OtpExpiresAt
    );
}