using BaseClinic.Business.Interfaces;

namespace BaseClinic.DataAccess.Repositories
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

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
