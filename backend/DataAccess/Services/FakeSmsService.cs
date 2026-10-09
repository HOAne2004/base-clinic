using BaseClinic.Business.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BaseClinic.DataAccess.Services
{
    /// <summary>
    /// SMS Service giả lập — log OTP ra console thay vì gửi SMS thật.
    /// Dùng cho môi trường dev/học tập.
    /// </summary>
    public class FakeSmsService : ISmsService
    {
        private readonly ILogger<FakeSmsService> _logger;

        public FakeSmsService(ILogger<FakeSmsService> logger)
        {
            _logger = logger;
        }

        public Task SendOtpAsync(
            string phoneNumber,
            string otpCode,
            string purpose,
            CancellationToken cancellationToken = default)
        {
            _logger.LogWarning(
                "════════ [FAKE SMS] ════════\n" +
                "  To      : {PhoneNumber}\n" +
                "  Purpose : {Purpose}\n" +
                "  OTP Code: {OtpCode}\n" +
                "════════════════════════════",
                phoneNumber, purpose, otpCode);

            return Task.CompletedTask;
        }
    }
}