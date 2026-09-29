using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ClinicDbContext _context;
        public PatientRepository (ClinicDbContext context)
        {
            _context = context;
        }
        public void Add(Patient patient)
        {
            _context.Patients.Add(patient);
        }
        public async Task<Patient?> GetPrimaryPatientByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            // Hồ sơ chính chủ đơn giản là hồ sơ có gắn AccountId khớp với phiên đăng nhập
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);
        }

        public async Task<Patient?> GetDependentPatientByIdAsync(Guid dependentId, Guid observerAccountId, CancellationToken cancellationToken = default)
        {
            // Kiểm tra hồ sơ tồn tại VÀ tài khoản đang đăng nhập (ObserverAccountId) 
            // có giấy phép (Delegation) ở trạng thái Accepted
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == dependentId &&
                    _context.PatientDelegations.Any(d =>
                        d.TargetPatientId == dependentId &&
                        d.ObserverAccountId == observerAccountId &&
                        d.Status == DelegationStatus.Accepted),
                    cancellationToken);
        }
        public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Patient?> GetByPatientCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientCode == code, cancellationToken);
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
    }
}
