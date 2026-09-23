namespace BaseClinic.Business.Interfaces
{
    public interface IPatientCodeGenerator
    {
        Task<string> GenerateAsync(CancellationToken cancellation = default);
    }
}
