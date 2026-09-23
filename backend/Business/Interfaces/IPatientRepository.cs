using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Interfaces
{
    public interface IPatientRepository
    {
        void Add(Patient patient);
        
        // Lấy hồ sơ chính (Primary) dựa trên AccountId đang đăng nhập
        Task<Patient?> GetPrimaryPatientByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);

        // Kiểm tra xem hồ sơ phụ (Dependent) có thực sự thuộc quyền quản lý của hồ sơ chính không
        Task<Patient?> GetDependentPatientByIdAsync(Guid dependentId, Guid primaryPatientId, CancellationToken cancellationToken = default);

        Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
    }
}
