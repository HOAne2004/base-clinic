using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class PatientDelegationRepository
        : Repository<PatientDelegation>, IPatientDelegationRepository
    {
        public PatientDelegationRepository(ClinicDbContext context) : base(context) { }

        public async Task<PatientDelegation?> GetActiveByTargetAndObserverAsync(
            Guid targetPatientId,
            Guid observerAccountId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d =>
                    d.TargetPatientId == targetPatientId
                    && d.ObserverAccountId == observerAccountId
                    && d.Status == DelegationStatus.Accepted,
                    cancellationToken);
        }

        public async Task<List<PatientDelegation>> GetActiveByTargetPatientIdAsync(
            Guid targetPatientId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.TargetPatientId == targetPatientId
                         && d.Status == DelegationStatus.Accepted)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PatientDelegation>> GetActiveByObserverAccountIdAsync(
            Guid observerAccountId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.ObserverAccountId == observerAccountId
                         && d.Status == DelegationStatus.Accepted)
                .ToListAsync(cancellationToken);
        }
    }
}