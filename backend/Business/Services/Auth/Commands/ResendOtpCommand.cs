using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Auth;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    /// <summary>
    /// Command gửi lại OTP xác thực SĐT.
    /// 
    /// LUỒNG:
    /// 1. Tìm Account theo AccountId.
    /// 2. Check status phải là Inactive (chưa xác thực).
    /// 3. Vô hiệu hóa toàn bộ OTP cũ đang pending.
    /// 4. Sinh OTP mới.
    /// 5. Gửi SMS.
    /// 6. Trả về thông tin để FE hiển thị.
    /// </summary>
    public record ResendOtpCommand(
        Guid AccountId
    ) : IRequest<ResendOtpResultDto>;

    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, ResendOtpResultDto>
    {
        private const int OtpExpiryMinutes = 5;
        private const int ResendCooldownSeconds = 60;

        private readonly IAccountRepository _accountRepository;
        private readonly IOtpCodeRepository _otpCodeRepository;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;

        public ResendOtpCommandHandler(
            IAccountRepository accountRepository,
            IOtpCodeRepository otpCodeRepository,
            ISmsService smsService,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _otpCodeRepository = otpCodeRepository;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResendOtpResultDto> Handle(
            ResendOtpCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm Account ====================
            var account = await _accountRepository
                .GetByIdAsync(request.AccountId, cancellationToken);

            if (account == null)
                throw new NotFoundException("Không tìm thấy tài khoản.");

            // ==================== BƯỚC 2: Check status ====================
            if (account.Status != AccountStatus.Inactive)
                throw new DomainException(
                    "Tài khoản không ở trạng thái chờ xác thực. " +
                    "Vui lòng đăng nhập hoặc liên hệ quản trị viên.");

            if (account.IsPhoneNumberVerified)
                throw new DomainException("Số điện thoại đã được xác thực.");

            // ==================== BƯỚC 3: CHECK COOLDOWN 60s ====================
            var lastCreatedAt = await _otpCodeRepository
                .GetLatestCreatedTimeAsync(account.Id, OtpPurpose.VerifyPhone, cancellationToken);

            if (lastCreatedAt.HasValue)
            {
                var elapsed = DateTimeOffset.UtcNow - lastCreatedAt.Value;
                var remaining = TimeSpan.FromSeconds(ResendCooldownSeconds) - elapsed;

                if (remaining > TimeSpan.Zero)
                    throw new DomainException(
                        $"Vui lòng đợi {Math.Ceiling(remaining.TotalSeconds)} giây " +
                        $"trước khi yêu cầu mã mới.");
            }

            // ==================== BƯỚC 4: Vô hiệu hóa OTP cũ ====================
            await _otpCodeRepository.InvalidateAllPendingAsync(
                account.Id, OtpPurpose.VerifyPhone, cancellationToken);

            // ==================== BƯỚC 5: Sinh OTP mới ====================
            var otp = OtpCode.Create(
                accountId: account.Id,
                purpose: OtpPurpose.VerifyPhone,
                channel: OtpChannel.Sms,
                expiryMinutes: OtpExpiryMinutes);

            _otpCodeRepository.Add(otp);

            // ==================== BƯỚC 6: Lưu ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ==================== BƯỚC 7: Gửi SMS ====================
            await _smsService.SendOtpAsync(
                phoneNumber: account.PhoneNumber,
                otpCode: otp.Code,
                purpose: "xác thực số điện thoại",
                cancellationToken: cancellationToken);

            // ==================== BƯỚC 8: Response ====================
            return new ResendOtpResultDto(
                PhoneNumber: account.PhoneNumber,
                OtpExpiresAt: otp.ExpiresAt);
        }
    }
}