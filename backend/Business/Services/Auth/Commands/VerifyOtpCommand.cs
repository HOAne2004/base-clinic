using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Account;
using BaseClinic.Business.Models.Auth;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    /// <summary>
    /// Command xác thực OTP sau khi đăng ký.
    /// 
    /// LUỒNG:
    /// 1. Tìm Account theo AccountId.
    /// 2. Check status (phải là Inactive — chờ xác thực).
    /// 3. Tìm OTP valid theo AccountId + Purpose.
    /// 4. So sánh code:
    ///    - Sai → IncrementAttempts → throw.
    ///    - Đúng → MarkAsUsed → VerifyPhone → Activate.
    /// 5. Sinh token (giống Login).
    /// 6. Trả về AuthResultDto.
    /// </summary>
    public record VerifyOtpCommand(
        Guid AccountId,
        string OtpCode,
        string? IpAddress = null,
        string? DeviceInfo = null
    ) : IRequest<AuthResultDto>;

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResultDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOtpCodeRepository _otpCodeRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyOtpCommandHandler(
            IAccountRepository accountRepository,
            IOtpCodeRepository otpCodeRepository,
            IJwtProvider jwtProvider,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _otpCodeRepository = otpCodeRepository;
            _jwtProvider = jwtProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResultDto> Handle(
            VerifyOtpCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm Account ====================
            var account = await _accountRepository
                .GetByIdWithRefreshTokensAsync(request.AccountId, cancellationToken);

            if (account == null)
                throw new NotFoundException("Không tìm thấy tài khoản.");

            // ==================== BƯỚC 2: Check status ====================
            if (account.Status == AccountStatus.Active)
                throw new DomainException("Tài khoản đã được xác thực trước đó.");

            if (account.Status == AccountStatus.Suspended)
                throw new DomainException(
                    "Tài khoản đã bị tạm ngưng. Vui lòng liên hệ quản trị viên.");

            // ==================== BƯỚC 3: Tìm OTP valid ====================
            var otp = await _otpCodeRepository.GetLatestValidAsync(
                account.Id, OtpPurpose.VerifyPhone, cancellationToken);

            if (otp == null)
                throw new DomainException(
                    "Mã OTP không tồn tại hoặc đã hết hạn. Vui lòng yêu cầu mã mới.");

            // ==================== BƯỚC 4: So sánh code ====================
            if (otp.Code != request.OtpCode)
            {
                otp.IncrementAttempts();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var remaining = otp.MaxAttempts - otp.Attempts;
                if (remaining > 0)
                    throw new DomainException(
                        $"Mã OTP không đúng. Còn {remaining} lần thử.");
                else
                    throw new DomainException(
                        "Mã OTP đã bị vô hiệu hóa do nhập sai quá nhiều lần. " +
                        "Vui lòng yêu cầu mã mới.");
            }

            // ==================== BƯỚC 5: OTP đúng → Verify + Activate ====================
            otp.MarkAsUsed();
            account.VerifyPhoneNumber();
            account.Activate();
            // Revoke tất cả refresh token cũ (nếu có) — đảm bảo session cũ bị hủy
            account.RevokeAllRefreshTokens("Đã xác thực SĐT — các phiên cũ bị hủy.");
            
            // ==================== BƯỚC 6: Lấy role codes ====================
            var roles = await _accountRepository
                .GetRoleCodesByAccountIdAsync(account.Id, cancellationToken);

            // ==================== BƯỚC 7: Sinh refresh token ====================
            string refreshToken = _jwtProvider.GenerateRefreshToken();
            string hashedRefreshToken = _jwtProvider.HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken(
                accountId: account.Id,
                tokenHash: hashedRefreshToken,
                expiresAt: _jwtProvider.GetRefreshTokenExpiry(),
                deviceInfo: request.DeviceInfo,
                ipAddress: request.IpAddress,
                userAgent: null);

            account.AddRefreshToken(refreshTokenEntity);

            // ==================== BƯỚC 8: Sinh access token ====================
            var tokenUser = new TokenUser(
                account.Id,
                account.FullName,
                account.PhoneNumber,
                account.SecurityStamp,
                refreshTokenEntity.Id,
                roles);

            string accessToken = _jwtProvider.GenerateAccessToken(tokenUser);

            // ==================== BƯỚC 9: Ghi vết login ====================
            account.RecordLogin(
                DateTimeOffset.UtcNow,
                request.IpAddress,
                request.DeviceInfo);

            // ==================== BƯỚC 10: Lưu ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ==================== BƯỚC 11: Response ====================
            var userInfo = new AccountPublicInfoDto(
                account.Id,
                account.FullName,
                account.Email,
                account.PhoneNumber,
                account.Status,
                account.IsEmailVerified,
                account.IsPhoneNumberVerified,
                roles);

            return new AuthResultDto(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                User: userInfo,
                RequiresPasswordChange: account.MustChangePasswordOnNextLogin);
        }
    }
}