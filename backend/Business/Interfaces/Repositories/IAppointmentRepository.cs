using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces.Repositories
{
    /// <summary>
    /// Repository cho Appointment.
    /// Kế thừa IRepository&lt;Appointment&gt; để có sẵn CRUD.
    /// Chỉ khai báo thêm các method đặc thù.
    /// </summary>
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        /// <summary>
        /// Kiểm tra bệnh nhân có lịch hẹn đè lên khoảng thời gian không.
        /// Bỏ qua lịch Cancelled/Completed.
        /// </summary>
        Task<bool> HasOverlappingAppointmentAsync(
            Guid patientId,
            DateTime date,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kiểm tra bác sĩ có lịch hẹn đè lên khoảng thời gian không.
        /// Bỏ qua lịch Cancelled/Completed.
        /// </summary>
        Task<bool> HasOverlappingDoctorAppointmentAsync(
            Guid doctorId,
            DateTime date,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default);
    }
}