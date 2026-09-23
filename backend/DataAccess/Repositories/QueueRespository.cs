using BaseClinic.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using BaseClinic.Domain.Entities;

namespace BaseClinic.DataAccess.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly ClinicDbContext _context;
        public QueueRepository(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<Queue?> GetQueueByDepartmentAndDateAsync(Guid departmentId, DateTime date, CancellationToken cancellation = default)
        {
            return await _context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.DepartmentId == departmentId && q.QueueDate == date);
        }
        public void Add(Queue queue)
        {
            _context.Queues.Add(queue);
        }


        public async Task<string> GenerateQueueCodeAsync(Guid departmentId, DateTime date, CancellationToken cancellationToken = default)
        {
            var prefix = $"Q-{date:yyyyMMdd}";
            var count = await _context.Queues
                .Where(q => q.QueueCode.StartsWith(prefix))
                .CountAsync(cancellationToken);

            var sequence = (count + 1).ToString("D3");
            return $"{prefix}-{sequence}";
        }
        public async Task<Queue?> GetActiveQueueAsync(Guid departmentId, DateTime date, CancellationToken cancellation)
        {
            return await _context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.DepartmentId == departmentId
                && q.QueueDate == date.Date
                && q.Status == Domain.Enums.QueueStatus.Open, cancellation);
        }
    }
}
