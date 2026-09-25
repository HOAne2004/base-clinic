using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ClinicDbContext _context;
        public AccountRepository(ClinicDbContext context)
        {
            _context = context;
        }

        public void Add (Account account)
        {
            _context.Accounts.Add(account);
        }
        public async Task<Account?> GetAccountByPhoneNumberAsync(string phoneNumber, CancellationToken cancellation)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber, cancellation);
        }

        public async Task<List<string>> GetPermissionsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.Id == accountId)
                .SelectMany(a => a.AccountRoles)
                .Select(ar => ar.Role)
                .SelectMany(r => r.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetRolesByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.Id == accountId)
                .SelectMany(a => a.AccountRoles)
                .Select(ar => ar.Role.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Account?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }
    }
}
