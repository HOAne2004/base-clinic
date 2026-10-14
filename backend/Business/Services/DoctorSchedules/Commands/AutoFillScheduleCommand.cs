using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Auto-fill nhiều ca cho 1 bác sĩ trong 1 khoảng ngày.
    /// 
    /// CẤU HÌNH:
    /// - Ca sáng: MorningStart → MorningEnd (bắt buộc).
    /// - Ca chiều: AfternoonStart → AfternoonEnd (optional).
    /// - DaysOfWeek: chỉ tạo các ngày match (VD: T2-T6).
    /// 
    /// XỬ LÝ MỖI NGÀY:
    /// - Skip nếu DayOfWeek không match.
    /// - Skip nếu ngày đó đã có Schedule + SkipExistingDays = true.
    /// - Nếu SkipExistingDays = false → check overlap cụ thể rồi mới skip/tạo.
    /// 
    /// TRẢ VỀ: Danh sách ngày đã tạo + ngày đã skip.
    /// </summary>
    public record AutoFillScheduleCommand(AutoFillScheduleDto Dto)
        : IRequest<AutoFillResultDto>;

    public class AutoFillScheduleCommandHandler
        : IRequestHandler<AutoFillScheduleCommand, AutoFillResultDto>
    {
        private const int MaxDaysAhead = 90;
        private const int MaxRangeDays = 90;

        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AutoFillScheduleCommandHandler(
            IDoctorScheduleRepository scheduleRepo,
            IDoctorRepository doctorRepo,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepo = scheduleRepo;
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<AutoFillResultDto> Handle(
            AutoFillScheduleCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // ==================== VALIDATE INPUT ====================
            ValidateInput(dto);

            // ==================== LOAD DOCTOR + DEPARTMENT ====================
            var (doctor, department) = await LoadAndValidateDoctorDepartmentAsync(
                dto, cancellationToken);

            // ==================== VALIDATE AGAINST DEPARTMENT HOURS ====================
            ValidateAgainstDepartmentHours(dto, department);

            // ==================== LOAD EXISTING SCHEDULES ====================
            var existingSchedules = await _scheduleRepo.GetQueryable()
                .Where(s => s.DoctorId == doctor.Id
                         && s.WorkDate >= dto.StartDate
                         && s.WorkDate <= dto.EndDate
                         && s.Status != DoctorScheduleStatus.Cancelled)
                .ToListAsync(cancellationToken);

            var existingByDate = existingSchedules
                .GroupBy(s => s.WorkDate)
                .ToDictionary(g => g.Key, g => g.ToList());

            // ==================== LOOP QUA TỪNG NGÀY ====================
            var createdDates = new List<DateOnly>();
            var skippedDates = new List<DateOnly>();
            var schedulesToAdd = new List<Domain.Entities.DoctorSchedule>();

            bool hasAfternoon = dto.AfternoonStart.HasValue
                             && dto.AfternoonEnd.HasValue;

            var current = dto.StartDate;
            while (current <= dto.EndDate)
            {
                // Skip nếu DayOfWeek không match
                if (!dto.DaysOfWeek.Contains(current.DayOfWeek))
                {
                    current = current.AddDays(1);
                    continue;
                }

                bool hasExisting = existingByDate.ContainsKey(current);

                // Nếu có schedule + SkipExistingDays → skip
                if (hasExisting && dto.SkipExistingDays)
                {
                    skippedDates.Add(current);
                    current = current.AddDays(1);
                    continue;
                }

                // Nếu có schedule + không skip → check overlap từng ca
                if (hasExisting)
                {
                    var existing = existingByDate[current];

                    bool morningOverlap = existing.Any(e =>
                        dto.MorningStart < e.EndTime
                        && dto.MorningEnd > e.StartTime);

                    if (morningOverlap)
                    {
                        skippedDates.Add(current);
                        current = current.AddDays(1);
                        continue;
                    }

                    if (hasAfternoon)
                    {
                        bool afternoonOverlap = existing.Any(e =>
                            dto.AfternoonStart!.Value < e.EndTime
                            && dto.AfternoonEnd!.Value > e.StartTime);

                        if (afternoonOverlap)
                        {
                            skippedDates.Add(current);
                            current = current.AddDays(1);
                            continue;
                        }
                    }
                }

                // ==================== TẠO CA SÁNG ====================
                var morningSchedule = new Domain.Entities.DoctorSchedule(
                    doctorId: doctor.Id,
                    workDate: current,
                    startTime: dto.MorningStart,
                    endTime: dto.MorningEnd,
                    slotDurationMinutes: dto.SlotDurationMinutes,
                    maxPatientPerSlot: dto.MaxPatientPerSlot,
                    onlineBookingEnabled: dto.OnlineBookingEnabled,
                    note: dto.Note);

                morningSchedule.GenerateItems();
                schedulesToAdd.Add(morningSchedule);

                // ==================== TẠO CA CHIỀU (nếu có) ====================
                if (hasAfternoon)
                {
                    var afternoonSchedule = new Domain.Entities.DoctorSchedule(
                        doctorId: doctor.Id,
                        workDate: current,
                        startTime: dto.AfternoonStart!.Value,
                        endTime: dto.AfternoonEnd!.Value,
                        slotDurationMinutes: dto.SlotDurationMinutes,
                        maxPatientPerSlot: dto.MaxPatientPerSlot,
                        onlineBookingEnabled: dto.OnlineBookingEnabled,
                        note: dto.Note);

                    afternoonSchedule.GenerateItems();
                    schedulesToAdd.Add(afternoonSchedule);
                }

                createdDates.Add(current);
                current = current.AddDays(1);
            }

            // ==================== SAVE ====================
            if (schedulesToAdd.Any())
            {
                foreach (var s in schedulesToAdd)
                    _scheduleRepo.Add(s);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return new AutoFillResultDto(
                CreatedSchedules: schedulesToAdd.Count,
                SkippedDays: skippedDates.Count,
                CreatedDates: createdDates,
                SkippedDates: skippedDates);
        }

        // ==================== VALIDATION HELPERS ====================

        private static void ValidateInput(AutoFillScheduleDto dto)
        {
            // 1. Date range
            if (dto.StartDate > dto.EndDate)
                throw new DomainException(
                    "StartDate phải trước hoặc bằng EndDate.");

            var totalDays = dto.EndDate.DayNumber - dto.StartDate.DayNumber + 1;
            if (totalDays > MaxRangeDays)
                throw new DomainException(
                    $"Khoảng thời gian không vượt quá {MaxRangeDays} ngày.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (dto.StartDate < today)
                throw new DomainException("StartDate không thể ở quá khứ.");

            var maxDate = today.AddDays(MaxDaysAhead);
            if (dto.EndDate > maxDate)
                throw new DomainException(
                    $"Chỉ có thể tạo lịch trong vòng {MaxDaysAhead} ngày tới.");

            // 2. DaysOfWeek
            if (dto.DaysOfWeek == null || !dto.DaysOfWeek.Any())
                throw new DomainException("Phải chọn ít nhất 1 ngày trong tuần.");

            // 3. Ca sáng
            if (dto.MorningStart >= dto.MorningEnd)
                throw new DomainException(
                    "Giờ bắt đầu ca sáng phải trước giờ kết thúc.");

            // 4. Ca chiều — cùng null hoặc cùng có giá trị
            bool hasAfternoonStart = dto.AfternoonStart.HasValue;
            bool hasAfternoonEnd = dto.AfternoonEnd.HasValue;

            if (hasAfternoonStart != hasAfternoonEnd)
                throw new DomainException(
                    "Ca chiều phải có cả AfternoonStart và AfternoonEnd " +
                    "hoặc để trống cả hai.");

            if (hasAfternoonStart)
            {
                if (dto.AfternoonStart!.Value >= dto.AfternoonEnd!.Value)
                    throw new DomainException(
                        "Giờ bắt đầu ca chiều phải trước giờ kết thúc.");

                if (dto.AfternoonStart!.Value < dto.MorningEnd)
                    throw new DomainException(
                        "Ca chiều phải bắt đầu sau khi ca sáng kết thúc.");
            }

            // 5. Slot duration chia hết thời gian
            var morningMinutes = (dto.MorningEnd.ToTimeSpan()
                - dto.MorningStart.ToTimeSpan()).TotalMinutes;

            if (morningMinutes % dto.SlotDurationMinutes != 0)
                throw new DomainException(
                    "Thời gian ca sáng phải chia hết cho thời lượng slot.");

            if (hasAfternoonStart)
            {
                var afternoonMinutes = (dto.AfternoonEnd!.Value.ToTimeSpan()
                    - dto.AfternoonStart!.Value.ToTimeSpan()).TotalMinutes;

                if (afternoonMinutes % dto.SlotDurationMinutes != 0)
                    throw new DomainException(
                        "Thời gian ca chiều phải chia hết cho thời lượng slot.");
            }
        }

        private async Task<(Doctor Doctor, Department Department)> LoadAndValidateDoctorDepartmentAsync(
            AutoFillScheduleDto dto,
            CancellationToken cancellationToken)
        {
            // Load Doctor
            var doctor = await _doctorRepo.GetByIdAsync(dto.DoctorId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            if (!doctor.IsActive)
                throw new DomainException(
                    "Không thể tạo ca cho bác sĩ đang tạm ngưng.");

            if (dto.OnlineBookingEnabled && !doctor.IsAvailableForOnline)
                throw new DomainException(
                    "Bác sĩ không cho phép đặt lịch trực tuyến. " +
                    "Vui lòng bật IsAvailableForOnline cho bác sĩ trước.");

            // Load Department
            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy khoa.");

            if (!department.IsActive)
                throw new DomainException(
                    "Không thể tạo ca cho khoa đang tạm ngưng.");

            if (dto.OnlineBookingEnabled && !department.OnlineBookingEnabled)
                throw new DomainException(
                    "Khoa không cho phép đặt lịch trực tuyến. " +
                    "Vui lòng bật OnlineBookingEnabled cho khoa trước.");

            return (doctor, department);
        }

        private static void ValidateAgainstDepartmentHours(
            AutoFillScheduleDto dto,
            Department department)
        {
            if (department.Is24Hours)
                return;

            if (dto.MorningStart < department.OpeningTime)
                throw new DomainException(
                    $"Ca sáng phải bắt đầu sau giờ mở cửa ({department.OpeningTime}).");

            // Nếu có ca chiều → check end của ca chiều; ngược lại check end ca sáng
            var latestEnd = dto.AfternoonEnd ?? dto.MorningEnd;

            if (latestEnd > department.ClosingTime)
                throw new DomainException(
                    $"Ca phải kết thúc trước giờ đóng cửa ({department.ClosingTime}).");
        }
    }
}