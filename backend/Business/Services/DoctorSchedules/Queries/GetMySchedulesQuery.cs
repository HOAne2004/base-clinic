using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Queries
{
    /// <summary>
    /// Doctor tự xem lịch của mình.
    /// URL: GET /api/doctor-schedules/me
    /// 
    /// Default: từ hôm nay đến 30 ngày tới.
    /// </summary>
    public record GetMySchedulesQuery(
        Guid AccountId,
        DateOnly? FromDate = null,
        DateOnly? ToDate = null
    ) : IRequest<List<DoctorScheduleListDto>>;

    public class GetMySchedulesQueryHandler
        : IRequestHandler<GetMySchedulesQuery, List<DoctorScheduleListDto>>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetMySchedulesQueryHandler(
            IDoctorRepository doctorRepo,
            IDoctorScheduleRepository scheduleRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _scheduleRepo = scheduleRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<List<DoctorScheduleListDto>> Handle(
            GetMySchedulesQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Tìm doctor theo AccountId
            var doctor = await _doctorRepo.GetByAccountIdAsync(
                request.AccountId, cancellationToken)
                ?? throw new NotFoundException(
                    "Tài khoản này chưa được liên kết với hồ sơ bác sĩ.");

            // 2. Xác định date range
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var fromDate = request.FromDate ?? today;
            var toDate = request.ToDate ?? today.AddDays(7);

            // 3. Load schedules
            var schedules = await _scheduleRepo.GetQueryable()
                .Include(s => s.Items)
                .Where(s => s.DoctorId == doctor.Id
                         && s.WorkDate >= fromDate
                         && s.WorkDate <= toDate)
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync(cancellationToken);

            // 4. Load thông tin phụ
            var account = await _accountRepo.GetByIdAsync(
                doctor.AccountId, cancellationToken);

            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken);

            // 5. Map
            return schedules
                .Select(s => DoctorScheduleMapper.ToListItemDto(
                    schedule: s,
                    doctor: doctor,
                    doctorFullName: account?.FullName ?? string.Empty,
                    doctorCode: doctor.DoctorCode,
                    department: department))
                .ToList();
        }
    }
}