using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class QueueRepository : Repository<Queue>, IQueueRepository
    {
        public QueueRepository(ClinicDbContext context) : base(context) { }

        public async Task<Queue?> GetQueueByDepartmentAndDateAsync(
            Guid departmentId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q =>
                    q.DepartmentId == departmentId &&
                    q.QueueDate == date,
                    cancellationToken);
        }

        public async Task<string> GenerateQueueCodeAsync(
            Guid departmentId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            var prefix = $"Q-{date:yyyyMMdd}";
            var count = await _dbSet
                .Where(q => q.QueueCode.StartsWith(prefix))
                .CountAsync(cancellationToken);

            var sequence = (count + 1).ToString("D3");
            return $"{prefix}-{sequence}";
        }

        public async Task<Queue?> GetActiveQueueAsync(
            Guid departmentId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q =>
                    q.DepartmentId == departmentId &&
                    q.QueueDate == date.Date &&
                    q.Status == QueueStatus.Open,
                    cancellationToken);
        }
    }
}