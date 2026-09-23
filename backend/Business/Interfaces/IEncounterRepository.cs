using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IEncounterRepository
    {
        void Add(Encounter encounter);
        Task<string> GenerateEncounterCodeAsync(CancellationToken cancellationToken = default);
    }
}
