
using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ClinicDbContext _context;
        public DepartmentRepository(ClinicDbContext context) {
            _context = context;
        }

        public void Add(Department department)
        {
            _context.Departments.Add(department);
        }
        public async Task<bool> IsCodeExistAsync(string departmentCode, CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentCode == departmentCode, cancellationToken);
            
        }
    }
}
