using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class PatientDelegationRepository : IPatientDelegationRepository
    {
        private readonly ClinicDbContext _context;
        public PatientDelegationRepository(ClinicDbContext context)
        {
            _context = context; 
        }
        public async Task<PatientDelegation?> GetByIdAsync(Guid delegationId, CancellationToken cancellation)
        {
            return await _context.PatientDelegations
                .FirstOrDefaultAsync(pd => pd.Id == delegationId);
        }

        public void AddDelegation(PatientDelegation delegation)
        {
            _context.PatientDelegations.Add(delegation);
        }

        public async Task<bool> CheckDelegationExistAsync(Guid targetPatientId, Guid accountId, CancellationToken cancellationToken)
        {
            return await _context.PatientDelegations
                .AnyAsync(d => d.TargetPatientId == targetPatientId &&
                d.ObserverAccountId == accountId &&
                (d.Status == DelegationStatus.Pending || d.Status == DelegationStatus.Accepted)
                , cancellationToken);
        }

        // Hàm tìm xem ai đang có quyền quản lý hồ sơ này (Lấy người đầu tiên)
        public async Task<Guid?> GetCreatorAccountIdByTargetPatientIdAsync(Guid targetPatientId, CancellationToken cancellationToken = default)
        {
            return await _context.PatientDelegations
                .Where(d => d.TargetPatientId == targetPatientId && d.Status == DelegationStatus.Accepted)
                .Select(d => d.ObserverAccountId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
