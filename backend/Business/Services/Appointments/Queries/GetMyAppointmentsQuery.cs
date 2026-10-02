using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using BaseClinic.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BaseClinic.Business.Services.Appointments.Queries
{
    public record GetMyAppointmentsQuery(
        DateTime? FromDate,
        DateTime? ToDate,
        AppointmentStatus? Status
    ) : IRequest<List<AppointmentListItemDto>>;

    public class GetMyAppointmentsQueryHandler : IRequestHandler<GetMyAppointmentsQuery, List<AppointmentListItemDto>>
    {
        private readonly ClinicDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetMyAppointmentsQueryHandler(ClinicDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUser = currentUserService;
        }

        public async Task<List<AppointmentListItemDto>> Handle(
            GetMyAppointmentsQuery request,
            CancellationToken cancellationToken)
        {
            var accountId = _currentUser.AccountId
                ?? throw new UnauthorizedAccessException("Không thể xác thực danh tính người dùng.");

            var myPatientIds = await _context.Patients
                .Where(p => p.AccountId == accountId)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            var delegatedPatientIds = await _context.PatientDelegations
                .Where(pd =>
                    pd.ObserverAccountId == accountId &&
                    pd.Status == DelegationStatus.Accepted)
                .Select(pd => pd.TargetPatientId)
                .ToListAsync(cancellationToken);

            var allowedPatientIds = myPatientIds
                .Union(delegatedPatientIds)
                .ToList();

            var query =
                from a in _context.Appointments
                join p in _context.Patients
                    on a.PatientId equals p.Id

                join d in _context.Departments
                    on a.DepartmentId equals d.Id into deptGroup
                from d in deptGroup.DefaultIfEmpty()

                    // 1. Join bảng Doctors
                join doc in _context.Doctors
                    on a.RequestedDoctorId equals doc.Id into docGroup
                from doc in docGroup.DefaultIfEmpty()

                    // 2. Join tiếp bảng Accounts để lấy FullName của Bác sĩ
                join acc in _context.Accounts
                    on doc.AccountId equals acc.Id into accGroup
                from acc in accGroup.DefaultIfEmpty()

                where allowedPatientIds.Contains(a.PatientId)
                   || a.CreatedBy == accountId

                select new
                {
                    Appointment = a,
                    Patient = p,
                    Department = d,
                    DoctorName = acc != null ? acc.FullName : null // Lấy tên trực tiếp từ Account
                };

            if (request.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.Appointment.AppointmentDate >=
                    request.FromDate.Value.Date);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(x =>
                    x.Appointment.AppointmentDate <=
                    request.ToDate.Value.Date);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Appointment.Status == request.Status.Value);
            }

            return await query
                .OrderByDescending(x => x.Appointment.AppointmentDate)
                .ThenByDescending(x => x.Appointment.StartTime)
                .Select(x => new AppointmentListItemDto(
                    x.Appointment.Id,
                    x.Appointment.PatientId,
                    x.Patient.FullName ?? string.Empty,
                    x.Department != null
                        ? x.Department.Name
                        : null,
                    x.DoctorName, // Truyền trực tiếp biến đã lấy ở trên
                    x.Appointment.AppointmentDate,
                    x.Appointment.StartTime,
                    x.Appointment.EndTime,
                    x.Appointment.Status.ToString()
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
