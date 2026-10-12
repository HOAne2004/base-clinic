using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        /// <summary>Kiểm tra mã khoa đã tồn tại chưa.</summary>
        Task<bool> IsCodeExistAsync(
            string departmentCode,
            CancellationToken cancellationToken = default);
    }
}