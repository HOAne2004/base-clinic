using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Account;
using BaseClinic.Business.Models.Auth;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    public record LoginCommand(
        string PhoneNumber,
        string Password,
        string? IpAddress = null,
        string? DeviceInfo = null
    ) : IRequest<AuthResultDto>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(
            IAccountRepository accountRepository,
            IJwtProvider jwtProvider,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm Account ====================
            var account = await _accountRepository
                .GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

            if (account == null)
                throw new UnauthorizedAccessException(
                    "Số điện thoại hoặc mật khẩu không chính xác.");

            // ==================== BƯỚC 2: Check lockout ====================
            if (account.IsLockedOut())
            {
                var remaining = account.LockoutEnd!.Value - DateTimeOffset.UtcNow;
                throw new UnauthorizedAccessException(
                    $"Tài khoản đang bị khóa do nhập sai quá nhiều lần. " +
                    $"Vui lòng thử lại sau {Math.Ceiling(remaining.TotalMinutes)} phút.");
            }

            // ==================== BƯỚC 3: Check status ====================
            if (account.Status == AccountStatus.Suspended)
                throw new UnauthorizedAccessException(
                    "Tài khoản đã bị tạm ngưng. Vui lòng liên hệ quản trị viên.");

            if (account.Status == AccountStatus.Inactive)
                throw new UnauthorizedAccessException(
                    "Tài khoản chưa được kích hoạt. Vui lòng xác thực SĐT trước.");


            // ==================== BƯỚC 4: Verify password ====================
            bool isPasswordValid = _passwordHasher.Verify(request.Password, account.PasswordHash);
            if (!isPasswordValid)
            {
                account.RecordFailedLogin(
                    request.IpAddress, request.DeviceInfo, "Sai mật khẩu");

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var remainingAttempts = 5 - account.FailedLoginAttempts;
                if (remainingAttempts > 0)
                    throw new UnauthorizedAccessException(
                        $"Số điện thoại hoặc mật khẩu không chính xác. " +
                        $"Còn {remainingAttempts} lần thử.");
                else
                    throw new UnauthorizedAccessException(
                        "Tài khoản đã bị khóa 15 phút do nhập sai quá nhiều lần.");
            }

            // ==================== BƯỚC 5: Lấy roles ====================
            var roles = await _accountRepository
                .GetRoleCodesByAccountIdAsync(account.Id, cancellationToken);

            // ==================== BƯỚC 6: Sinh refresh token ====================
            string refreshToken = _jwtProvider.GenerateRefreshToken();
            string hashedRefreshToken = _jwtProvider.HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken(
                accountId: account.Id,
                tokenHash: hashedRefreshToken,
                expiresAt: _jwtProvider.GetRefreshTokenExpiry(),
                deviceInfo: request.DeviceInfo,
                ipAddress: request.IpAddress,
                userAgent: null);

            // ⚡ PHẢI add refresh token trước để có Id làm SessionId
            account.AddRefreshToken(refreshTokenEntity);


            // ==================== BƯỚC 7: Sinh access token ====================
            var tokenUser = new TokenUser(
                account.Id,
                account.FullName,
                account.PhoneNumber,
                account.SecurityStamp,
                refreshTokenEntity.Id,     // ← SessionId = RefreshToken.Id
                roles);

            string accessToken = _jwtProvider.GenerateAccessToken(tokenUser);

            // ==================== BƯỚC 8: Ghi vết đăng nhập ====================
            account.RecordLogin(
                DateTimeOffset.UtcNow,
                request.IpAddress,
                request.DeviceInfo);

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
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                User: userInfo,
                RequiresPasswordChange: account.MustChangePasswordOnNextLogin);
        }
    }
}