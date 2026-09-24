using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IDepartmentRepository
    {
        void Add(Department department);

        //Mã khoa là duy nhất
        Task<bool> IsCodeExistAsync(string departmentCode, CancellationToken cancellationToken = default);
        Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
