using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>Lấy Role kèm danh sách RolePermissions (eager load).</summary>
        Task<Role?> GetByIdWithPermissionsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>Lấy Role theo tên hiển thị (VD: "Quản trị viên").</summary>
        Task<Role?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>Lấy Role theo mã (VD: "ADMIN"). Dùng cho check role trong code.</summary>
        Task<Role?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách mã permission của một role theo Role.Code.
        /// Dùng cho RequirePermissionFilter — cache 10 phút.
        /// </summary>
        Task<HashSet<string>> GetPermissionCodesByRoleCodeAsync(
            string roleCode,
            CancellationToken cancellationToken = default);
    }
}
