using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    /// <summary>
    /// Command thu hồi 1 session (thiết bị) cụ thể.
    /// 
    /// LUỒNG:
    /// 1. Tìm RefreshToken theo SessionId.
    /// 2. Check token thuộc Account hiện tại.
    /// 3. Check token còn active.
    /// 4. Revoke.
    /// 
    /// LƯU Ý:
    /// - Revoke session khác → access token của session đó VẪN DÙNG ĐƯỢC
    ///   đến khi hết hạn (tối đa 15 phút).
    /// - Không đổi SecurityStamp (vì sẽ ảnh hưởng cả session hiện tại).
    /// - Nếu muốn vô hiệu ngay → dùng LogoutAllDevices.
    /// </summary>
    public record RevokeSessionCommand(
        Guid AccountId,
        Guid SessionId
    ) : IRequest<Unit>;

    public class RevokeSessionCommandHandler
        : IRequestHandler<RevokeSessionCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeSessionCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            RevokeSessionCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm token ====================
            var token = await _refreshTokenRepository
                .GetByIdAsync(request.SessionId, cancellationToken);

            if (token == null)
                throw new NotFoundException("Không tìm thấy phiên đăng nhập.");

            // ==================== BƯỚC 2: Check ownership ====================
            if (token.AccountId != request.AccountId)
                throw new DomainException("Phiên đăng nhập không thuộc tài khoản này.");

            // ==================== BƯỚC 3: Check active ====================
            if (!token.IsActive)
                throw new DomainException(
                    "Phiên đăng nhập đã kết thúc hoặc đã hết hạn.");

            // ==================== BƯỚC 4: Revoke ====================
            token.Revoke("Người dùng đăng xuất thiết bị này.");
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}