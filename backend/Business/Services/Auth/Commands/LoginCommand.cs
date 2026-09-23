using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    public class LoginCommand : IRequest<string>
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

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
            {
                Id = account.Id,
                FullName = account.FullName,
                Email = account.Email ?? string.Empty, // Đảm bảo không null nếu DB chưa bắt buộc
                SecurityStamp = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                Roles = roles,
                Permissions = permissions
            };

            // Bước 5: Gọi hàm đúc Token
            string accessToken = _jwtProvider.GenerateAccessToken(tokenUser);

            return accessToken;
        }
    }
}