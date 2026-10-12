using Azure.Core;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        /// <summary>Lấy Patient theo mã BN (VD: "BN2026100001").</summary>
        Task<Patient?> GetByCodeAsync(
            string patientCode,
            CancellationToken cancellationToken = default);

        /// <summary>Lấy Patient primary của 1 Account.</summary>
        Task<Patient?> GetPrimaryByAccountIdAsync(
            Guid accountId,
            CancellationToken cancellationToken = default);

        /// <summary>Lấy Patient theo Id kèm Account (để hiển thị SĐT, Email).</summary>
        Task<Patient?> GetByIdWithAccountAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Query BN kèm Account (dùng cho danh sách + search).
        /// </summary>
        IQueryable<Patient> GetQueryableWithAccount();

        /// <summary>
        /// Lấy Patient theo Id, kèm dependents (để chuyển khi merge).
        /// </summary>
        Task<Patient?> GetByIdWithDependentsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Đếm lịch sử (appointments + encounters) của Patient.
        /// Dùng để so sánh khi chọn survivor.
        /// </summary>
        Task<int> CountHistoryAsync(
            Guid patientId,
            CancellationToken cancellationToken = default);

        Task<List<Patient>> GetDependentsAsync(
            Guid primaryPatientId,
            CancellationToken cancellationToken = default);
    }
}
