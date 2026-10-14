using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Tạo 1 ca làm việc đơn.
    /// 
    /// VALIDATION:
    /// - Doctor tồn tại + active.
    /// - Department active (nếu OnlineBookingEnabled = true → department phải bật online).
    /// - Thời gian nằm trong Department.OpeningTime/ClosingTime (nếu không 24/24).
    /// - Không overlap với ca khác của cùng BS cùng ngày, HOẶC nếu overlap thì SlotDuration phải giống.
    /// - WorkDate trong tương lai (<= 90 ngày).
    /// </summary>
    public record CreateScheduleCommand(CreateScheduleDto Dto)
        : IRequest<Guid>;

    public class CreateScheduleCommandHandler
        : IRequestHandler<CreateScheduleCommand, Guid>
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduleCommandHandler(
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

        public async Task<Guid> Handle(
            CreateScheduleCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // ==================== BƯỚC 1: Validate DTO cơ bản ====================
            if (dto.StartTime == default(TimeOnly))
                throw new DomainException("StartTime không được để trống.");

            if (dto.EndTime == default(TimeOnly))
                throw new DomainException("EndTime không được để trống.");

            // ==================== BƯỚC 2: Load Doctor ====================
            var doctor = await _doctorRepo.GetByIdAsync(dto.DoctorId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            if (!doctor.IsActive)
                throw new DomainException(
                    "Không thể tạo ca cho bác sĩ đang tạm ngưng.");

            if (dto.OnlineBookingEnabled && !doctor.IsAvailableForOnline)
                throw new DomainException(
                    "Bác sĩ không cho phép đặt lịch trực tuyến. " +
                    "Vui lòng bật IsAvailableForOnline cho bác sĩ trước.");

            // ==================== BƯỚC 3: Load Department ====================
            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken)
                ?? throw new NotFoundException(
                    "Không tìm thấy khoa của bác sĩ.");

            if (!department.IsActive)
                throw new DomainException(
                    "Không thể tạo ca cho khoa đang tạm ngưng.");

            if (dto.OnlineBookingEnabled && !department.OnlineBookingEnabled)
                throw new DomainException(
                    "Khoa không cho phép đặt lịch trực tuyến. " +
                    "Vui lòng bật OnlineBookingEnabled cho khoa trước.");

            // ==================== BƯỚC 4: Validate thời gian với Department ====================
            if (!department.Is24Hours)
            {
                if (dto.StartTime < department.OpeningTime)
                    throw new DomainException(
                        $"Giờ bắt đầu ca phải sau giờ mở cửa ({department.OpeningTime}).");

                if (dto.EndTime > department.ClosingTime)
                    throw new DomainException(
                        $"Giờ kết thúc ca phải trước giờ đóng cửa ({department.ClosingTime}).");
            }

            // ==================== BƯỚC 5: Check overlap ====================
            var existingSchedules = await _scheduleRepo.GetQueryable()
                .Where(s => s.DoctorId == doctor.Id
                         && s.WorkDate == dto.WorkDate
                         && s.Status != DoctorScheduleStatus.Cancelled)
                .ToListAsync(cancellationToken);

            foreach (var existing in existingSchedules)
            {
                bool overlaps = dto.StartTime < existing.EndTime
                             && dto.EndTime > existing.StartTime;

                if (overlaps && existing.SlotDurationMinutes != dto.SlotDurationMinutes)
                {
                    throw new DomainException(
                        $"Ca bị overlap với ca hiện có ({existing.StartTime}-{existing.EndTime}) " +
                        $"nhưng thời lượng slot khác nhau " +
                        $"({existing.SlotDurationMinutes} vs {dto.SlotDurationMinutes} phút). " +
                        $"Vui lòng chọn cùng thời lượng slot.");
                }
            }

            // ==================== BƯỚC 6: Tạo Schedule ====================
            var schedule = new Domain.Entities.DoctorSchedule(
                doctorId: doctor.Id,
                workDate: dto.WorkDate,
                startTime: dto.StartTime,
                endTime: dto.EndTime,
                slotDurationMinutes: dto.SlotDurationMinutes,
                maxPatientPerSlot: dto.MaxPatientPerSlot,
                onlineBookingEnabled: dto.OnlineBookingEnabled,
                note: dto.Note);

            // Sinh slots
            schedule.GenerateItems();

            _scheduleRepo.Add(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return schedule.Id;
        }
    }
}