using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using BaseClinic.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BaseClinic.Business.Services.Appointments.Queries
{
    public record GetPatientAppointmentsQuery(
        Guid PatientId,
        DateTime? FromDate,
        DateTime? ToDate,
        AppointmentStatus? Status
    ) : IRequest<List<AppointmentListItemDto>>;

    public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, List<AppointmentListItemDto>>
    {
        private readonly ClinicDbContext _context;

        public GetPatientAppointmentsQueryHandler(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppointmentListItemDto>> Handle(
            GetPatientAppointmentsQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Kiểm tra sự tồn tại của bệnh nhân
            var patientExists = await _context.Patients
                .AsNoTracking()
                .AnyAsync(
                    p => p.Id == request.PatientId,
                    cancellationToken);

            if (!patientExists)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin bệnh nhân.");
            }

            // 2. Truy vấn danh sách lịch hẹn của bệnh nhân
            var query =
                from a in _context.Appointments.AsNoTracking()
                join p in _context.Patients
                    on a.PatientId equals p.Id

                join d in _context.Departments
                    on a.DepartmentId equals d.Id into deptGroup
                from d in deptGroup.DefaultIfEmpty()

                join doc in _context.Doctors
                    on a.RequestedDoctorId equals doc.Id into docGroup
                from doc in docGroup.DefaultIfEmpty()

                    // Join bảng Accounts để lấy họ tên Bác sĩ
                join acc in _context.Accounts
                    on doc.AccountId equals acc.Id into accGroup
                from acc in accGroup.DefaultIfEmpty()

                where a.PatientId == request.PatientId
                select new
                {
                    Appointment = a,
                    Patient = p,
                    Department = d,
                    DoctorName = acc != null ? acc.FullName : null // Trích xuất tên trực tiếp
                };

            // 3. Áp dụng các bộ lọc tìm kiếm
            if (request.FromDate.HasValue)
            {
                query = query.Where(q =>
                    q.Appointment.AppointmentDate >= request.FromDate.Value.Date);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(q =>
                    q.Appointment.AppointmentDate <= request.ToDate.Value.Date);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(q =>
                    q.Appointment.Status == request.Status.Value);
            }

            // 4. Map dữ liệu sang DTO
            return await query
                .OrderByDescending(q => q.Appointment.AppointmentDate)
                .ThenByDescending(q => q.Appointment.StartTime)
                .Select(q => new AppointmentListItemDto(
                    q.Appointment.Id,
                    q.Appointment.PatientId,
                    q.Patient.FullName ?? string.Empty,
                    q.Department != null ? q.Department.Name : null,
                    q.DoctorName, 
                    q.Appointment.AppointmentDate,
                    q.Appointment.StartTime,
                    q.Appointment.EndTime,
                    q.Appointment.Status.ToString() 
                ))
                .ToListAsync(cancellationToken);
        }
    }
}