using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IQueueRepository : IRepository<Queue>
    {
        Task<Queue?> GetQueueByDepartmentAndDateAsync(
            Guid departmentId,
            DateTime date,
            CancellationToken cancellation = default);

        Task<string> GenerateQueueCodeAsync(
            Guid departmentId,
            DateTime date,
            CancellationToken cancellationToken = default);

        Task<Queue?> GetActiveQueueAsync(
            Guid departmentId,
            DateTime date,
            CancellationToken cancellationToken = default);
    }
}