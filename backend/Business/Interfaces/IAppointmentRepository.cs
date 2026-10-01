using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Add(Appointment appointment);

        /// <summary>
        /// Kiểm tra xem bệnh nhân có lịch hẹn nào (chưa bị hủy/hoàn thành) đè lên khoảng thời gian yêu cầu không.
        /// </summary>
        Task<bool> HasOverlappingAppointmentAsync(
            Guid patientId,
            DateTime date,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken);

        /// <summary>
        /// Kiểm tra xem bác sĩ có lịch hẹn nào (chưa bị hủy/hoàn thành) đè lên khoảng thời gian yêu cầu không.
        /// </summary>
        Task<bool> HasOverlappingDoctorAppointmentAsync(
            Guid doctorId,
            DateTime date,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken);
    }
}
