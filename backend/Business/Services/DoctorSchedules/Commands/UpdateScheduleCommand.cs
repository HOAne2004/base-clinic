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
    /// Cập nhật ca làm việc.
    /// 
    /// ĐIỀU KIỆN:
    /// - Ca chưa Completed / Cancelled.
    /// - Chưa có slot nào bị book.
    /// 
    /// KHI UPDATE:
    /// - Xóa hết items cũ.
    /// - Sinh lại items mới theo cấu hình mới.
    /// </summary>
    public record UpdateScheduleCommand(
        Guid Id,
        UpdateScheduleDto Dto
    ) : IRequest<bool>;

    public class UpdateScheduleCommandHandler
        : IRequestHandler<UpdateScheduleCommand, bool>
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateScheduleCommandHandler(
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

        public async Task<bool> Handle(
            UpdateScheduleCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // ==================== BƯỚC 1: Load schedule + items ====================
            var schedule = await _scheduleRepo.GetByIdWithItemsAsync(
                request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            // ==================== BƯỚC 2: Load Doctor + Department ====================
            var doctor = await _doctorRepo.GetByIdAsync(
                schedule.DoctorId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy khoa.");

            // ==================== BƯỚC 3: Validate thời gian với Department ====================
            if (!department.Is24Hours)
            {
                if (dto.StartTime < department.OpeningTime)
                    throw new DomainException(
                        $"Giờ bắt đầu ca phải sau giờ mở cửa ({department.OpeningTime}).");

                if (dto.EndTime > department.ClosingTime)
                    throw new DomainException(
                        $"Giờ kết thúc ca phải trước giờ đóng cửa ({department.ClosingTime}).");
            }

            // ==================== BƯỚC 4: Update schedule (validate bên trong) ====================
            schedule.UpdateSchedule(
                startTime: dto.StartTime,
                endTime: dto.EndTime,
                slotDurationMinutes: dto.SlotDurationMinutes,
                maxPatientPerSlot: dto.MaxPatientPerSlot,
                onlineBookingEnabled: dto.OnlineBookingEnabled,
                note: dto.Note);

            // ==================== BƯỚC 5: Sinh lại items ====================
            schedule.GenerateItems();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}