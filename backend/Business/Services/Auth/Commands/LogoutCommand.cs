using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    // Định nghĩa Command nhận vào AccountId
    public record LogoutCommand(Guid AccountId) : IRequest;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy Account từ DB
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (account != null)
            {
                // 2. Thu hồi Refresh Token
                account.RevokeRefreshToken();

                // 3. Lưu vào DB
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Nếu dùng MediatR IRequest (không trả về kiểu dữ liệu), hàm sẽ kết thúc tại đây thay vì return Unit.Value
        }
    }
}