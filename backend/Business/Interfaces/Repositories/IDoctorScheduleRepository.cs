using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        /// <summary>
        /// Lấy Schedule kèm Items + Doctor.
        /// Dùng cho các query cần chi tiết hoặc slots.
        /// </summary>
        Task<DoctorSchedule?> GetByIdWithItemsAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}