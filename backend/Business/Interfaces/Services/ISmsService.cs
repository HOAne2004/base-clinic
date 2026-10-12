namespace BaseClinic.Business.Interfaces.Services
{
    /// <summary>
    /// Interface gửi SMS. Ban đầu dùng Fake implementation (log ra console).
    /// Sau này tích hợp provider thật (Twilio, VNPT, Viettel...).
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Gửi mã OTP đến số điện thoại.
        /// </summary>
        Task SendOtpAsync(
            string phoneNumber,
            string otpCode,
            string purpose,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gửi SMS thông báo chung (không phải OTP).
        /// </summary>
        Task SendNotificationAsync(
            string phoneNumber,
            string message,
            CancellationToken cancellationToken = default);
    }
}