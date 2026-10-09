using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<Account?> GetByPhoneNumberAsync(
            string phoneNumber,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.RefreshTokens)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber, cancellationToken);
        }

        public async Task<List<string>> GetRoleCodesByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.Id == accountId)
                .SelectMany(a => a.AccountRoles)
                .Select(ar => ar.Role.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetPermissionCodesByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.Id == accountId)
                .SelectMany(a => a.AccountRoles)
                .SelectMany(ar => ar.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}