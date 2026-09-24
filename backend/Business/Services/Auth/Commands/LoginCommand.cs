using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    public record LoginCommand
        ( string PhoneNumber, string Password): IRequest<string>;
    
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(
            IAccountRepository accountRepository,
            IJwtProvider jwtProvider,
            IPasswordHasher passwordHasher)
        {
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
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

            return accessToken;
        }
    }
}