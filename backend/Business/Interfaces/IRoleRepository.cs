using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
