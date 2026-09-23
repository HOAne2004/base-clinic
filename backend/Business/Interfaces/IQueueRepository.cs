using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IQueueRepository
    {
        Task<Queue?> GetQueueByDepartmentAndDateAsync(Guid departmentId, DateTime date, CancellationToken cancellation = default);
        void Add(Queue queue);
        Task<string> GenerateQueueCodeAsync(Guid departmentId, DateTime date, CancellationToken cancellationToken = default);
        Task<Queue?> GetActiveQueueAsync(Guid departmentId, DateTime date, CancellationToken cancellationToken = default);
    }
}
