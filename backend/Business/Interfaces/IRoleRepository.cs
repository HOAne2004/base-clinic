using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IRoleRepository
    {
        void Add(Role role);
        void Remove(Role role);
        Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
