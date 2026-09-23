using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IAccountRepository
    {
        void Add(Account account);
        Task<Account?> GetAccountByPhoneNumberAsync(string phoneNumber, CancellationToken cancellation);
        Task<List<string>> GetRolesByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<List<string>> GetPermissionsByAccountIdAsync (Guid accountId, CancellationToken cancellation = default);
    }
}
