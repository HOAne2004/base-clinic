namespace BaseClinic.Business.Interfaces.Services
{
    public interface IPatientCodeGenerator
    {
        Task<string> GenerateAsync(CancellationToken cancellation = default);
    }
}
