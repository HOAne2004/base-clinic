using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.DoctorSchedule;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Queries
{
    /// <summary>
    /// Admin xem tất cả ca làm việc.
    /// URL: GET /api/doctor-schedules
    /// </summary>
    public record GetAllSchedulesQuery(ScheduleSearchDto Search)
        : IRequest<PagedResultDto<DoctorScheduleListDto>>;

    public class GetAllSchedulesQueryHandler
        : IRequestHandler<GetAllSchedulesQuery, PagedResultDto<DoctorScheduleListDto>>
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetAllSchedulesQueryHandler(
            IDoctorScheduleRepository scheduleRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _scheduleRepo = scheduleRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<PagedResultDto<DoctorScheduleListDto>> Handle(
            GetAllSchedulesQuery request,
            CancellationToken cancellationToken)
        {
            var search = request.Search;

            // 1. Query với Include
            var query = _scheduleRepo.GetQueryable()
                .Include(s => s.Items)
                .Include(s => s.Doctor)
                .AsQueryable();

            if (search.DoctorId.HasValue)
                query = query.Where(s => s.DoctorId == search.DoctorId.Value);

            // Filter theo department qua Doctor.DepartmentId
            if (search.DepartmentId.HasValue)
                query = query.Where(s => s.Doctor.DepartmentId == search.DepartmentId.Value);

            if (search.FromDate.HasValue)
                query = query.Where(s => s.WorkDate >= search.FromDate.Value);

            if (search.ToDate.HasValue)
                query = query.Where(s => s.WorkDate <= search.ToDate.Value);

            if (search.Status.HasValue)
                query = query.Where(s => s.Status == search.Status.Value);

            if (search.OnlineBookingEnabled.HasValue)
                query = query.Where(s => s.OnlineBookingEnabled == search.OnlineBookingEnabled.Value);

            query = query
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.StartTime);

            // 2. Paging
            var total = await query.CountAsync(cancellationToken);

            var schedules = await query
                .Skip((search.PageIndex - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync(cancellationToken);

            // 3. Batch load Accounts
            var accountIds = schedules
                .Select(s => s.Doctor.AccountId)
                .Distinct()
                .ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // 4. Batch load Departments
            var departmentIds = schedules
                .Select(s => s.Doctor.DepartmentId)
                .Distinct()
                .ToList();

            var departments = await _departmentRepo.GetQueryable()
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d, cancellationToken);

            // 5. Map
            var items = schedules
                .Select(s => DoctorScheduleMapper.ToListItemDto(
                    schedule: s,
                    doctor: s.Doctor,
                    doctorFullName: accountNames.GetValueOrDefault(
                        s.Doctor.AccountId, string.Empty),
                    doctorCode: s.Doctor.DoctorCode,
                    department: departments.GetValueOrDefault(s.Doctor.DepartmentId)))
                .ToList();

            return new PagedResultDto<DoctorScheduleListDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = search.PageIndex,
                PageSize = search.PageSize
            };
        }
    }
}