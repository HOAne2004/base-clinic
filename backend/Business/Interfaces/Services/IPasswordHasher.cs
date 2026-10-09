namespace BaseClinic.Business.Interfaces.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string plainPassword, string hashedPassword);
    }
}
