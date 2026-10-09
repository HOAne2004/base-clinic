using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class PatientRepository : Repository<Patient>,IPatientRepository
    {
        public PatientRepository(ClinicDbContext context) : base(context) { }

        public async Task<Patient?> GetByCodeAsync(
            string patientCode,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.PatientCode == patientCode, cancellationToken);
        }

        public async Task<Patient?> GetPrimaryByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.AccountId == accountId && p.IsPrimary, cancellationToken);
        }
    }
}
