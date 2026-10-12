using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
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
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.AccountId == accountId && p.IsPrimary, cancellationToken);
        }

        public async Task<Patient?> GetByIdWithAccountAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public IQueryable<Patient> GetQueryableWithAccount()
            => _dbSet.AsNoTracking().Include(p => p.Account);

        public async Task<Patient?> GetByIdWithDependentsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Dependents)
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<int> CountHistoryAsync(
            Guid patientId,
            CancellationToken cancellationToken = default)
        {
            var appointmentCount = await _context.Appointments
                .CountAsync(a => a.PatientId == patientId, cancellationToken);

            var encounterCount = await _context.Encounters
                .CountAsync(e => e.PatientId == patientId, cancellationToken);

            return appointmentCount + encounterCount;
        }

        public async Task<List<Patient>> GetDependentsAsync(
            Guid primaryPatientId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.PrimaryPatientId == primaryPatientId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
