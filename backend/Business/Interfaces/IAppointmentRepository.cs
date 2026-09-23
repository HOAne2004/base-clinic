using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Add(Appointment appointment);
    }
}
