using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using BaseClinic.Domain.Enums;
using BaseClinic.Presentation.Middlewares;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BaseClinic.Business.Services.Appointments.Queries
{
    public record GetAppointmentsQuery(
         DateTime? FromDate,
         DateTime? ToDate,
         Guid? DepartmentId,
         Guid? DoctorId,
         Guid? PatientId,
         AppointmentStatus? Status
     ) : IRequest<List<AppointmentListItemDto>>;

    public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, List<AppointmentListItemDto>>
    {
        private readonly ClinicDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetAppointmentsQueryHandler(ClinicDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<AppointmentListItemDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var accountId = _currentUser.AccountId
                ?? throw new UnauthorizedAccessException("Không thể xác thực danh tính.");

            var query = from a in _context.Appointments.AsNoTracking()
                        join p in _context.Patients on a.PatientId equals p.Id
                        join d in _context.Departments on a.DepartmentId equals d.Id into deptGroup
                        from d in deptGroup.DefaultIfEmpty()
                        join doc in _context.Doctors on a.RequestedDoctorId equals doc.Id into docGroup
                        from doc in docGroup.DefaultIfEmpty()
                        join acc in _context.Accounts on doc.AccountId equals acc.Id into accGroup
                        from acc in accGroup.DefaultIfEmpty()
                        select new { a, p, d, DoctorName = acc != null ? acc.FullName : null };

            // --- 1. DATA SCOPE AUTHORIZATION ---
            bool isAdmin = _currentUser.IsInRole("Admin");
            bool isReceptionist = _currentUser.IsInRole("Receptionist");
            bool isDoctor = _currentUser.IsInRole("Doctor");

            if (!isAdmin && !isReceptionist)
            {
                if (isDoctor)
                {
                    var myDoctorId = await _context.Doctors
                        .AsNoTracking()
                        .Where(d => d.AccountId == accountId)
                        .Select(d => d.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    // Bác sĩ chỉ nhìn thấy lịch hẹn có RequestedDoctorId là chính mình
                    query = query.Where(q => q.a.RequestedDoctorId == myDoctorId);
                }
                else
                {
                    throw new ForbiddenException("Bạn không có thẩm quyền truy cập tính năng tra cứu tổng quát.");
                }
            }

            // --- 2. CLIENT FILTERS ---
            if (request.FromDate.HasValue && request.ToDate.HasValue && request.FromDate.Value.Date > request.ToDate.Value.Date)
            {
                throw new ArgumentException("Từ ngày (FromDate) không được lớn hơn Đến ngày (ToDate).");
            }

            if (request.FromDate.HasValue)
                query = query.Where(q => q.a.AppointmentDate >= request.FromDate.Value.Date);

            if (request.ToDate.HasValue)
                query = query.Where(q => q.a.AppointmentDate <= request.ToDate.Value.Date);

            if (request.DepartmentId.HasValue)
                query = query.Where(q => q.a.DepartmentId == request.DepartmentId.Value);

            // Chỉ Admin/Lễ tân mới có thể dùng tham số DoctorId để lọc tự do.
            if ((isAdmin || isReceptionist) && request.DoctorId.HasValue)
                query = query.Where(q => q.a.RequestedDoctorId == request.DoctorId.Value);

            if (request.PatientId.HasValue)
                query = query.Where(q => q.a.PatientId == request.PatientId.Value);

            if (request.Status.HasValue)
                query = query.Where(q => q.a.Status == request.Status.Value);

            // --- 3. PROJECTION ---
            return await query
                .OrderByDescending(q => q.a.AppointmentDate)
                .ThenByDescending(q => q.a.StartTime)
                .Select(q => new AppointmentListItemDto(
                    q.a.Id,
                    q.a.PatientId,
                    q.p.FullName ?? string.Empty,
                    q.d != null ? q.d.Name : null,
                    q.DoctorName,
                    q.a.AppointmentDate,
                    q.a.StartTime,
                    q.a.EndTime,
                    q.a.Status.ToString()
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
