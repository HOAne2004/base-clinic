using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByIdWithPermissionsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
        }

        public async Task<Role?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
        }

        public async Task<HashSet<string>> GetPermissionCodesByRoleCodeAsync(
            string roleCode,
            CancellationToken cancellationToken = default)
        {
            var codes = await _dbSet
                .Where(r => r.Code == roleCode && r.IsActive)
                .SelectMany(r => r.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToListAsync(cancellationToken);

            return codes.ToHashSet();
        }
    }
}