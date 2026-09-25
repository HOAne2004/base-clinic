using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Interfaces
{
    public interface IDoctorRepository
    {
        void Add(Doctor doctor);
        Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
