using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using BaseClinic.Domain.Enums;
using BaseClinic.Presentation.Middlewares;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Appointments.Queries
{
    public record GetAppointmentByIdQuery(
        Guid AppointmentId
    ) : IRequest<AppointmentDetailDto?>;

    public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDetailDto?>
    {
        private readonly ClinicDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetAppointmentByIdQueryHandler(
            ClinicDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<AppointmentDetailDto?> Handle(
            GetAppointmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var accountId = _currentUser.AccountId
                ?? throw new UnauthorizedAccessException("Không thể xác thực danh tính người dùng.");

            // Thêm AsNoTracking() và Join bảng Account để lấy tên Bác sĩ
            var result = await (
                from a in _context.Appointments.AsNoTracking()
                join p in _context.Patients on a.PatientId equals p.Id

                join d in _context.Departments on a.DepartmentId equals d.Id into deptGroup
                from d in deptGroup.DefaultIfEmpty()

                join doc in _context.Doctors on a.RequestedDoctorId equals doc.Id into docGroup
                from doc in docGroup.DefaultIfEmpty()

                join acc in _context.Accounts on doc.AccountId equals acc.Id into accGroup
                from acc in accGroup.DefaultIfEmpty()

                where a.Id == request.AppointmentId
                select new
                {
                    a,
                    p,
                    d,
                    DoctorName = acc != null ? acc.FullName : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return null;
            }

            // --- KIỂM TRA PHẠM VI TRUY CẬP (DATA SCOPE AUTHORIZATION) ---

            // 1. Hồ sơ bệnh nhân được liên kết với account hiện tại
            var isOwner = result.p.AccountId == accountId;

            // 2. Tài khoản hiện tại chính là người đã tạo lịch hẹn này
            var isCreator = result.a.CreatedBy == accountId;

            // 3. Tài khoản hiện tại được ủy quyền xem hồ sơ bệnh nhân này
            var isDelegated =
                !isOwner &&
                !isCreator &&
                await _context.PatientDelegations
                    .AsNoTracking()
                    .AnyAsync(pd =>
                        pd.ObserverAccountId == accountId &&
                        pd.TargetPatientId == result.a.PatientId &&
                        pd.Status == DelegationStatus.Accepted,
                        cancellationToken);

            // Bổ sung TODO cho Staff Scope sau này
            // var isAuthorizedStaff = _authorizationService.CheckStaffScope(...);

            if (!isOwner && !isCreator && !isDelegated)
            {
                throw new ForbiddenException("Bạn không có quyền truy cập lịch hẹn này.");
            }

            return new AppointmentDetailDto(
                result.a.Id,
                result.a.PatientId,
                result.p.FullName ?? string.Empty,
                result.a.DepartmentId,
                result.d?.Name,
                result.a.RequestedDoctorId,
                result.DoctorName,
                result.a.AppointmentDate,
                result.a.StartTime,
                result.a.EndTime,
                result.a.Reason,
                result.a.Status,
                result.a.CancelReason
            );
        }
    }
}