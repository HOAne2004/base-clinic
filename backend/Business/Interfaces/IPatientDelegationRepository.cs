using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IPatientDelegationRepository
    {
        Task<PatientDelegation?> GetByIdAsync( Guid delegationId, CancellationToken cancellationToken = default(CancellationToken));
        void AddDelegation(PatientDelegation delegation);
        Task<bool> CheckDelegationExistAsync(Guid targetPatientId, Guid observerAccountId, CancellationToken cancellationToken = default);
        Task<Guid?> GetCreatorAccountIdByTargetPatientIdAsync(Guid targetPatientId, CancellationToken cancellationToken = default);
    }
}
