using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ClinicDbContext context) : base(context) { }

        public async Task<bool> IsCodeExistAsync(
            string departmentCode,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(d => d.DepartmentCode == departmentCode, cancellationToken);
        }
    }
}