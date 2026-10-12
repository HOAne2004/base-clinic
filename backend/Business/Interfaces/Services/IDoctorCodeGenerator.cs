namespace BaseClinic.Business.Interfaces.Services
{
    public interface IDoctorCodeGenerator
    {
        Task<string> GenerateAsync(CancellationToken cancellation = default);
    }
}
