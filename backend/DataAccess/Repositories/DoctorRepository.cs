using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;

namespace BaseClinic.DataAccess.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicDbContext context) : base(context) { }

        // Không có method đặc thù — tất cả dùng base.
    }
}