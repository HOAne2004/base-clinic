using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IEncounterRepository : IRepository<Encounter>
    {
        /// <summary>Sinh mã Encounter — format ENC-yyyyMMdd-xxxx.</summary>
        Task<string> GenerateEncounterCodeAsync(
            CancellationToken cancellationToken = default);
    }
}