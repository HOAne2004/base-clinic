using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicDbContext context) : base(context) { }

        public async Task<bool> HasOverlappingAppointmentAsync(
            Guid patientId,
            DateTime date,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(a =>
                a.PatientId == patientId &&
                a.AppointmentDate.Date == date.Date &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Completed &&
                a.StartTime < endTime && a.EndTime > startTime,
                cancellationToken);
        }

        public async Task<bool> HasOverlappingDoctorAppointmentAsync(
            Guid doctorId,
            DateTime date,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(a =>
                a.RequestedDoctorId == doctorId &&
                a.AppointmentDate.Date == date.Date &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Completed &&
                a.StartTime < endTime && a.EndTime > startTime,
                cancellationToken);
        }
    }
}