using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Queries
{
    /// <summary>
    /// Admin xem lịch của 1 bác sĩ cụ thể.
    /// URL: GET /api/doctor-schedules/by-doctor/{doctorId}
    /// </summary>
    public record GetSchedulesByDoctorQuery(
        Guid DoctorId,
        ScheduleSearchDto Search
    ) : IRequest<PagedResultDto<DoctorScheduleListDto>>;

    public class GetSchedulesByDoctorQueryHandler
        : IRequestHandler<GetSchedulesByDoctorQuery, PagedResultDto<DoctorScheduleListDto>>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetSchedulesByDoctorQueryHandler(
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

        public async Task<PagedResultDto<DoctorScheduleListDto>> Handle(
            GetSchedulesByDoctorQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Validate doctor tồn tại
            var doctor = await _doctorRepo.GetByIdAsync(
                request.DoctorId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            // 2. Query schedules
            var search = request.Search;

            var query = _scheduleRepo.GetQueryable()
                .Include(s => s.Items)
                .Where(s => s.DoctorId == request.DoctorId);

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

            // 3. Paging
            var total = await query.CountAsync(cancellationToken);

            var schedules = await query
                .Skip((search.PageIndex - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync(cancellationToken);

            // 4. Load thông tin phụ
            var account = await _accountRepo.GetByIdAsync(
                doctor.AccountId, cancellationToken);

            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken);

            // 5. Map
            var items = schedules
                .Select(s => DoctorScheduleMapper.ToListItemDto(
                    schedule: s,
                    doctor: doctor,
                    doctorFullName: account?.FullName ?? string.Empty,
                    doctorCode: doctor.DoctorCode,
                    department: department))
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