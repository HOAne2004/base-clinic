using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Queries
{
    /// <summary>
    /// Lấy danh sách slot còn chỗ — dùng cho đặt lịch.
    /// URL: GET /api/doctor-schedules/available-slots
    /// 
    /// FILTER (tất cả optional):
    /// - DoctorId: chọn BS cụ thể
    /// - DepartmentId: chọn khoa (tự tìm BS trong khoa)
    /// - WorkDate: ngày khám (required)
    /// 
    /// CHỈ TRẢ VỀ SLOT:
    /// - Schedule.OnlineBookingEnabled = true
    /// - Schedule.Status = Available
    /// - Item.Status = Available và BookedCount < Capacity
    /// - Doctor.IsActive = true
    /// - Doctor.IsAvailableForOnline = true
    /// - Department.IsActive = true
    /// - Department.OnlineBookingEnabled = true
    /// </summary>
    public record GetAvailableSlotsQuery(
        DateOnly WorkDate,
        Guid? DoctorId = null,
        Guid? DepartmentId = null
    ) : IRequest<List<AvailableSlotDto>>;

    public class GetAvailableSlotsQueryHandler
        : IRequestHandler<GetAvailableSlotsQuery, List<AvailableSlotDto>>
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IAccountRepository _accountRepo;

        public GetAvailableSlotsQueryHandler(
            IDoctorScheduleRepository scheduleRepo,
            IDepartmentRepository departmentRepo,
            IAccountRepository accountRepo)
        {
            _scheduleRepo = scheduleRepo;
            _departmentRepo = departmentRepo;
            _accountRepo = accountRepo;
        }

        public async Task<List<AvailableSlotDto>> Handle(
            GetAvailableSlotsQuery request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Query schedules ====================
            var query = _scheduleRepo.GetQueryable()
                .Include(s => s.Items)
                .Include(s => s.Doctor)
                .Where(s => s.WorkDate == request.WorkDate
                         && s.Status == DoctorScheduleStatus.Available
                         && s.OnlineBookingEnabled
                         && s.Doctor.IsActive
                         && s.Doctor.IsAvailableForOnline);

            if (request.DoctorId.HasValue)
                query = query.Where(s => s.DoctorId == request.DoctorId.Value);

            if (request.DepartmentId.HasValue)
                query = query.Where(s => s.Doctor.DepartmentId == request.DepartmentId.Value);

            var schedules = await query.ToListAsync(cancellationToken);

            if (!schedules.Any())
                return new List<AvailableSlotDto>();

            // ==================== BƯỚC 2: Validate Departments ====================
            // Chỉ giữ lại schedules có Department active + online enabled
            var departmentIds = schedules
                .Select(s => s.Doctor.DepartmentId)
                .Distinct()
                .ToList();

            var departments = await _departmentRepo.GetQueryable()
                .Where(d => departmentIds.Contains(d.Id)
                         && d.IsActive
                         && d.OnlineBookingEnabled)
                .ToDictionaryAsync(d => d.Id, d => d, cancellationToken);

            // Filter schedules — chỉ giữ schedule có department hợp lệ
            var validSchedules = schedules
                .Where(s => departments.ContainsKey(s.Doctor.DepartmentId))
                .ToList();

            if (!validSchedules.Any())
                return new List<AvailableSlotDto>();

            // ==================== BƯỚC 3: Batch load Accounts ====================
            var accountIds = validSchedules
                .Select(s => s.Doctor.AccountId)
                .Distinct()
                .ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // ==================== BƯỚC 4: Flatten items + Filter available ====================
            var availableSlots = new List<AvailableSlotDto>();

            foreach (var schedule in validSchedules)
            {
                var department = departments[schedule.Doctor.DepartmentId];
                var doctorName = accountNames.GetValueOrDefault(
                    schedule.Doctor.AccountId, string.Empty);

                // Chỉ lấy item còn chỗ
                var availableItems = schedule.Items
                    .Where(i => i.IsAvailableForBooking())
                    .OrderBy(i => i.StartTime);

                foreach (var item in availableItems)
                {
                    availableSlots.Add(DoctorScheduleMapper.ToAvailableSlotDto(
                        item: item,
                        schedule: schedule,
                        doctor: schedule.Doctor,
                        doctorFullName: doctorName,
                        department: department));
                }
            }

            // ==================== BƯỚC 5: Sort ====================
            return availableSlots
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.DoctorId)
                .ToList();
        }
    }
}