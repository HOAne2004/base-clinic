using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    /// <summary>
    /// Repository cho Doctor.
    /// Không có method đặc thù nào ở giai đoạn hiện tại.
    /// </summary>
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    }
}