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
    /// Command đổi refresh token cũ lấy access token mới.
    /// 
    /// LUỒNG (Token Rotation):
    /// 1. Hash refresh token nhận được.
    /// 2. Tìm trong DB kèm Account + RefreshTokens.
    /// 3. Check:
    ///    - Token tồn tại.
    ///    - Chưa bị revoke.
    ///    - Chưa hết hạn.
    ///    - Account còn Active.
    /// 4. Revoke token cũ (rotation — mỗi refresh token chỉ dùng 1 lần).
    /// 5. Sinh token mới (access + refresh).
    /// 6. Trả về AuthResultDto.
    /// 
    /// BẢO MẬT: Nếu phát hiện refresh token cũ đã bị dùng lại → revoke toàn bộ
    /// refresh token của Account (dấu hiệu token bị đánh cắp).
    /// </summary>
    public record RefreshTokenCommand(
        string RefreshToken,
        string? IpAddress = null,
        string? DeviceInfo = null
    ) : IRequest<AuthResultDto>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IAccountRepository accountRepository,
            IJwtProvider jwtProvider,
            IUnitOfWork unitOfWork,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResultDto> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Hash token ====================
            string tokenHash = _jwtProvider.HashToken(request.RefreshToken);

            // ==================== BƯỚC 2: Tìm token trong DB ====================
            var token = await _refreshTokenRepository
                .GetByTokenHashWithAccountAsync(tokenHash, cancellationToken);

            if (token == null)
                throw new UnauthorizedAccessException(
                    "Refresh token không hợp lệ.");

            // ==================== BƯỚC 3: Check token còn valid ====================
            if (token.RevokedAt != null)
            {
                // BẢO MẬT: Log warning để điều tra
                _logger.LogWarning(
                    "⚠️ Phát hiện refresh token cũ được dùng lại. " +
                    "AccountId: {AccountId}, TokenId: {TokenId}, RevokedAt: {RevokedAt}, " +
                    "RevokeReason: {RevokeReason}, IP: {IpAddress}, Device: {DeviceInfo}",
                    token.AccountId,
                    token.Id,
                    token.RevokedAt,
                    token.RevokeReason,
                    request.IpAddress,
                    request.DeviceInfo);

                // Revoke toàn bộ token của account để bảo vệ.
                token.Account.RevokeAllRefreshTokens(
                    "Phát hiện refresh token cũ được dùng lại. Toàn bộ phiên bị hủy.");

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedAccessException(
                    "Refresh token đã bị thu hồi. Vui lòng đăng nhập lại.");
            }

            if (token.ExpiresAt <= DateTimeOffset.UtcNow)
                throw new UnauthorizedAccessException(
                    "Refresh token đã hết hạn. Vui lòng đăng nhập lại.");

            // ==================== BƯỚC 4: Check Account ====================
            var account = token.Account;

            if (account.Status == AccountStatus.Suspended)
                throw new UnauthorizedAccessException(
                    "Tài khoản đã bị tạm ngưng. Vui lòng liên hệ quản trị viên.");

            if (account.Status == AccountStatus.Inactive)
                throw new UnauthorizedAccessException(
                    "Tài khoản chưa được kích hoạt.");

            // ==================== BƯỚC 5: Rotate — revoke token cũ ====================
            token.Revoke("Đã sử dụng để lấy access token mới (rotation).");

            // ==================== BƯỚC 6: Sinh token mới ====================
            string newRefreshToken = _jwtProvider.GenerateRefreshToken();
            string hashedNewRefreshToken = _jwtProvider.HashToken(newRefreshToken);

            var newRefreshTokenEntity = new RefreshToken(
                accountId: account.Id,
                tokenHash: hashedNewRefreshToken,
                expiresAt: _jwtProvider.GetRefreshTokenExpiry(),
                deviceInfo: request.DeviceInfo ?? token.DeviceInfo,
                ipAddress: request.IpAddress ?? token.IpAddress,
                userAgent: token.UserAgent);

            account.AddRefreshToken(newRefreshTokenEntity);

            // ==================== BƯỚC 7: Lấy role codes ====================
            var roles = await _accountRepository
                .GetRoleCodesByAccountIdAsync(account.Id, cancellationToken);

            // ==================== BƯỚC 8: Sinh access token mới ====================
            var tokenUser = new TokenUser(
                account.Id,
                account.FullName,
                account.PhoneNumber,
                account.SecurityStamp,
                newRefreshTokenEntity.Id,   // SessionId = RefreshToken.Id mới
                roles);

            string newAccessToken = _jwtProvider.GenerateAccessToken(tokenUser);

            // ==================== BƯỚC 9: Lưu ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ==================== BƯỚC 10: Response ====================
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
                AccessToken: newAccessToken,
                RefreshToken: newRefreshToken,
                User: userInfo,
                RequiresPasswordChange: account.MustChangePasswordOnNextLogin);
        }
    }
}