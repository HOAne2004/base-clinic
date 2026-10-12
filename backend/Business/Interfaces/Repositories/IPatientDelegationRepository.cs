using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IPatientDelegationRepository : IRepository<PatientDelegation>
    {
        /// <summary>
        /// Lấy delegation active giữa 1 Target Patient và 1 Observer Account.
        /// Dùng cho Unlink, Revoke.
        /// </summary>
        Task<PatientDelegation?> GetActiveByTargetAndObserverAsync(
            Guid targetPatientId,
            Guid observerAccountId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy tất cả delegation active của 1 Target Patient.
        /// </summary>
        Task<List<PatientDelegation>> GetActiveByTargetPatientIdAsync(
            Guid targetPatientId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy tất cả delegation active mà 1 Observer Account đang có.
        /// </summary>
        Task<List<PatientDelegation>> GetActiveByObserverAccountIdAsync(
            Guid observerAccountId,
            CancellationToken cancellationToken = default);
    }
}