using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    public record LoginCommand
        (string PhoneNumber, string Password) : IRequest<AuthResultDto>;

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
            // Bước 1: Tìm Account theo số điện thoại 
            var account = await _accountRepository.GetAccountByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

            if (account == null)
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác.");

            // Bước 2: Xác thực mật khẩu thông qua IPasswordHasher
            bool isPasswordValid = _passwordHasher.Verify(request.Password, account.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác.");

            // Bước 3: Nếu mật khẩu đúng, gọi Repository để lấy danh sách Roles và Permissions
            var roles = await _accountRepository.GetRolesByAccountIdAsync(account.Id, cancellationToken);
            var permissions = await _accountRepository.GetPermissionsByAccountIdAsync(account.Id, cancellationToken);

            // Bước 4: Khởi tạo model TokenUser 
            var tokenUser = new TokenUser
            (
                account.Id,
                account.FullName,
                account.Email ?? string.Empty,
                Guid.NewGuid(),
                Guid.NewGuid(),
                roles,
                permissions
                );

            // Bước 5: Gọi hàm đúc Token
            string accessToken = _jwtProvider.GenerateAccessToken(tokenUser);
            string refreshToken = _jwtProvider.GenerateRefreshToken();

            // Buoc 6: Băm RefreshToken để bảo mật và cập nhật vào Account
            string hashedRefreshToken = _jwtProvider.HashToken(refreshToken);
            account.UpdateRefreshToken(hashedRefreshToken, _jwtProvider.GetRefreshTokenExpiry());

            // Buoc 7: Ghi vết đăng nhập
            account.RecordLogin(DateTimeOffset.UtcNow);

            // Buoc 8: Luu thay doi
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userInfo = new AccountPublicInfoDto(
            account.Id,
            account.FullName,
            account.Email,
            account.PhoneNumber,
            account.Status,
            account.IsEmailVerified,
            account.IsPhoneNumberVerified,
            roles.ToList()
        );

            return new AuthResultDto(accessToken,refreshToken, userInfo);
        }
    }
}