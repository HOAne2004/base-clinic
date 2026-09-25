namespace BaseClinic.Business.Interfaces
{
    public interface IDoctorCodeGenerator
    {
        Task<string> GenerateAsync(CancellationToken cancellation = default);
    }
}
