using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class DoctorScheduleRepository
        : Repository<DoctorSchedule>, IDoctorScheduleRepository
    {
        public DoctorScheduleRepository(ClinicDbContext context) : base(context) { }

        public async Task<DoctorSchedule?> GetByIdWithItemsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Items)
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
    }
}