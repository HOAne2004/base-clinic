using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using MediatR;
using System.Security.Claims;
using System.Threading;

namespace BaseClinic.Business.Services.Auth.Commands
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<TokenRefreshResultDto>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenRefreshResultDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(IAccountRepository accountRepository, IJwtProvider jwtProvider, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<TokenRefreshResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Giải mã AccessToken cũ (bỏ qua lỗi hết hạn) để lấy AccountId
            var principal = _jwtProvider.GetPrincipalFromExpiredToken(request.AccessToken);
            var accountIdString = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountIdString) || !Guid.TryParse(accountIdString, out Guid accountId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");

            // 2. Tìm Account trong DB
            var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
            if (account == null || account.Status != Domain.Enums.AccountStatus.Active)
                throw new UnauthorizedAccessException("Tài khoản không tồn tại hoặc đã bị khóa.");

            // 3. Kiểm tra Refresh Token
            var hashedIncomingToken = _jwtProvider.HashToken(request.RefreshToken);
            if (account.RefreshTokenHash != hashedIncomingToken || account.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
            {
                // Cảnh giác bảo mật: Nếu token sai hoặc hết hạn, nên thu hồi luôn để an toàn
                account.RevokeRefreshToken();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedAccessException("Refresh Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.");
            }

            // 4. Mọi thứ OK -> Cấp lại bộ Token mới tinh
            var roles = await _accountRepository.GetRolesByAccountIdAsync(account.Id, cancellationToken);
            var permissions = await _accountRepository.GetPermissionsByAccountIdAsync(account.Id, cancellationToken);
            var tokenUser = new TokenUser(account.Id, account.FullName, account.Email ?? "", Guid.NewGuid(), Guid.NewGuid(), roles, permissions);

            string newAccessToken = _jwtProvider.GenerateAccessToken(tokenUser);
            string newRefreshToken = _jwtProvider.GenerateRefreshToken();

            // 5. Cập nhật Token mới vào DB (Xoay vòng - Rotation)
            account.UpdateRefreshToken(_jwtProvider.HashToken(newRefreshToken), _jwtProvider.GetRefreshTokenExpiry());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TokenRefreshResultDto(newAccessToken, newRefreshToken);
        }
    }
}
