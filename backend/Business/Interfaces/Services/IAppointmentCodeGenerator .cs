namespace BaseClinic.Business.Interfaces.Services
{
    public interface IAppointmentCodeGenerator
    {
        Task<string> GenerateAsync(CancellationToken ct = default);
    }
}
