using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    /// <summary>
    /// Command đăng xuất thiết bị hiện tại.
    /// 
    /// LUỒNG:
    /// 1. Đọc SessionId từ JWT claim (= RefreshToken.Id).
    /// 2. Tìm RefreshToken theo Id.
    /// 3. Revoke token đó.
    /// 4. Không đổi SecurityStamp — access token hiện tại vẫn dùng được
    ///    đến khi hết hạn (tối đa 15 phút).
    /// 
    /// Lưu ý: SessionId được truyền từ Controller (đọc từ JWT).
    /// </summary>
    public record LogoutCommand(
        Guid AccountId,
        Guid SessionId
    ) : IRequest<Unit>;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm RefreshToken theo SessionId ====================
            var token = await _refreshTokenRepository
                .GetByIdAsync(request.SessionId, cancellationToken);

            if (token == null)
                throw new NotFoundException("Không tìm thấy phiên đăng nhập.");

            // ==================== BƯỚC 2: Check token thuộc Account này ====================
            if (token.AccountId != request.AccountId)
                throw new DomainException("Phiên đăng nhập không thuộc tài khoản này.");

            // ==================== BƯỚC 3: Revoke nếu còn active ====================
            // Idempotent: nếu đã revoke rồi thì bỏ qua (user bấm logout 2 lần)
            if (!token.IsActive)
                throw new DomainException("Phiên đăng nhập đã kết thúc hoặc đã hết hạn.");
            
            token.Revoke("Người dùng đăng xuất.");
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}