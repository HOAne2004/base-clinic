using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ClinicDbContext _context;
        public RoleRepository (ClinicDbContext context)
        {
            _context = context;
        }
        public async Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
        }
    }
}
