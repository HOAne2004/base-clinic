using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicDbContext context) : base(context) { }

        public async Task<Doctor?> GetByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.AccountId == accountId, cancellationToken);
        }
    }
}