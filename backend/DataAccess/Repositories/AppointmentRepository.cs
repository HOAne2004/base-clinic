using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ClinicDbContext _context;

        public AppointmentRepository (ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }
    }
}
