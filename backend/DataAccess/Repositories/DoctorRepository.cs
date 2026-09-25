using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BaseClinic.DataAccess.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ClinicDbContext _context;
        public DoctorRepository(ClinicDbContext context)
        {
            _context = context;
        }

        public void Add(Doctor doctor)
        {
             _context.Doctors.Add(doctor);
        }


        public async Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }
    }
}
