using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
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
        public async Task<bool> HasOverlappingAppointmentAsync(Guid patientId, DateTime date, TimeOnly startTime, TimeOnly endTime, CancellationToken cancellationToken)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.AppointmentDate.Date == date.Date &&
                // Trạng thái lịch hẹn không bị hủy hoặc đã xong
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Completed &&
                // Logic kiểm tra giao nhau (Overlap) của 2 khoảng thời gian
                a.StartTime < endTime && a.EndTime > startTime,
                cancellationToken);
        }

        public async Task<bool> HasOverlappingDoctorAppointmentAsync(Guid doctorId, DateTime date, TimeOnly startTime, TimeOnly endTime, CancellationToken cancellationToken)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.RequestedDoctorId == doctorId &&
                a.AppointmentDate.Date == date.Date &&
                // Trạng thái lịch hẹn không bị hủy hoặc đã xong
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Completed &&
                // Logic kiểm tra giao nhau (Overlap) của 2 khoảng thời gian
                a.StartTime < endTime && a.EndTime > startTime,
                cancellationToken);
        }
    }
}
