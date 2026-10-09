using BaseClinic.Business.Interfaces.Services;

namespace BaseClinic.DataAccess.Repositories
{
    public class PasswordHasher : IPasswordHasher
    {
        // đổi mật khẩu plain text (user nhập) thành chuỗi hash
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // xác thực mật khẩu nhập vào với hash đã lưu
        public bool Verify(string plainPassword, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
