using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Queries
{
    /// <summary>
    /// Lịch làm việc của tất cả BS trong 1 khoa vào 1 ngày — cho Lễ tân.
    /// URL: GET /api/doctor-schedules/day?departmentId=...&workDate=...
    /// </summary>
    public record GetDoctorDayScheduleQuery(
        Guid DepartmentId,
        DateOnly WorkDate
    ) : IRequest<DoctorDayScheduleDto>;

    public class GetDoctorDayScheduleQueryHandler
        : IRequestHandler<GetDoctorDayScheduleQuery, DoctorDayScheduleDto>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IAccountRepository _accountRepo;

        public GetDoctorDayScheduleQueryHandler(
            IDepartmentRepository departmentRepo,
            IDoctorScheduleRepository scheduleRepo,
            IAccountRepository accountRepo)
        {
            _departmentRepo = departmentRepo;
            _scheduleRepo = scheduleRepo;
            _accountRepo = accountRepo;
        }

        public async Task<DoctorDayScheduleDto> Handle(
            GetDoctorDayScheduleQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Validate department tồn tại
            var department = await _departmentRepo.GetByIdAsync(
                request.DepartmentId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy khoa.");

            // 2. Lấy tất cả schedules của khoa trong ngày
            var schedules = await _scheduleRepo.GetQueryable()
                .Include(s => s.Items)
                .Include(s => s.Doctor)
                .Where(s => s.Doctor.DepartmentId == request.DepartmentId
                         && s.WorkDate == request.WorkDate)
                .OrderBy(s => s.DoctorId)
                .ThenBy(s => s.StartTime)
                .ToListAsync(cancellationToken);

            if (!schedules.Any())
            {
                return new DoctorDayScheduleDto(
                    WorkDate: request.WorkDate,
                    DepartmentId: department.Id,
                    DepartmentName: department.Name,
                    Doctors: new List<DoctorDayItemDto>());
            }

            // 3. Batch load Account names
            var accountIds = schedules
                .Select(s => s.Doctor.AccountId)
                .Distinct()
                .ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // 4. Group theo Doctor
            var doctorGroups = schedules
                .GroupBy(s => s.DoctorId)
                .Select(g =>
                {
                    var firstSchedule = g.First();
                    var doctor = firstSchedule.Doctor;
                    var doctorName = accountNames.GetValueOrDefault(
                        doctor.AccountId, string.Empty);

                    return new DoctorDayItemDto(
                        DoctorId: doctor.Id,
                        DoctorName: doctorName,
                        DoctorCode: doctor.DoctorCode,
                        Avatar: doctor.Avatar,
                        Title: doctor.Title,
                        Specialization: doctor.Specialization,
                        Schedules: g
                            .OrderBy(s => s.StartTime)
                            .Select(DoctorScheduleMapper.ToSummaryItemDto)
                            .ToList());
                })
                .OrderBy(d => d.DoctorName)
                .ToList();

            return new DoctorDayScheduleDto(
                WorkDate: request.WorkDate,
                DepartmentId: department.Id,
                DepartmentName: department.Name,
                Doctors: doctorGroups);
        }
    }
}