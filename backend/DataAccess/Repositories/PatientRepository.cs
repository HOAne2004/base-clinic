using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
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
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.AccountId == accountId && p.PrimaryPatientId == null, cancellationToken);
        }
        public async Task<Patient?> GetDependentPatientByIdAsync(Guid dependentId, Guid primaryPatientId, CancellationToken cancellationToken = default)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == dependentId && p.PrimaryPatientId == primaryPatientId, cancellationToken);
        }
        public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
    }
}
