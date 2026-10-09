using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    /// <summary>
    /// Command đăng xuất khỏi TẤT CẢ thiết bị.
    /// 
    /// LUỒNG:
    /// 1. Tìm Account theo AccountId.
    /// 2. Gọi Account.LogoutAllDevices():
    ///    - Đổi SecurityStamp → vô hiệu hóa MỌI access token hiện có.
    ///    - Revoke tất cả refresh token.
    /// 3. Lưu.
    /// 
    /// Hệ quả:
    /// - User phải login lại trên MỌI thiết bị.
    /// - Kể cả access token đang dùng cũng bị vô hiệu (nhờ SecurityStamp middleware).
    /// </summary>
    public record LogoutAllDevicesCommand(
        Guid AccountId
    ) : IRequest<Unit>;

    public class LogoutAllDevicesCommandHandler
        : IRequestHandler<LogoutAllDevicesCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutAllDevicesCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            LogoutAllDevicesCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm Account ====================
            // Cần Include RefreshTokens để RevokeAllRefreshTokens hoạt động.
            var account = await _accountRepository
                .GetByIdWithRefreshTokensAsync(request.AccountId, cancellationToken);

            if (account == null)
                throw new NotFoundException("Không tìm thấy tài khoản.");

            // ==================== BƯỚC 2: Logout all ====================
            account.LogoutAllDevices();

            // ==================== BƯỚC 3: Lưu ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}