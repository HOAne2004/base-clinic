using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Xóa mềm bác sĩ.
    /// 
    /// ĐIỀU KIỆN:
    /// - Không có Appointment nào (bất kỳ status nào).
    /// - DoctorSchedule: tự động hủy hết (chưa Completed/Cancelled).
    /// </summary>
    public record DeleteDoctorCommand(
        Guid Id,
        Guid? DeletedByAccountId = null    // audit ai xóa
    ) : IRequest<bool>;

    public class DeleteDoctorCommandHandler
        : IRequestHandler<DeleteDoctorCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IRepository<DoctorSchedule> _scheduleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDoctorCommandHandler(
            IDoctorRepository doctorRepo,
            IAppointmentRepository appointmentRepo,
            IRepository<DoctorSchedule> scheduleRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _appointmentRepo = appointmentRepo;
            _scheduleRepo = scheduleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteDoctorCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm doctor ====================
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            // ==================== BƯỚC 2: Check không có Appointment ====================
            var hasAppointments = await _appointmentRepo.GetQueryable()
                .AnyAsync(a => a.RequestedDoctorId == doctor.Id, cancellationToken);

            if (hasAppointments)
                throw new DomainException(
                    "Không thể xóa bác sĩ vì vẫn còn lịch hẹn. " +
                    "Vui lòng xử lý các lịch hẹn trước.");

            // ==================== BƯỚC 3: Hủy toàn bộ DoctorSchedule ====================
            var schedules = await _scheduleRepo.GetQueryableWithTracking()
                .Where(s => s.DoctorId == doctor.Id)
                .ToListAsync(cancellationToken);

            var cancelReason = "Bác sĩ đã bị xóa khỏi hệ thống.";
            var cancelledBy = request.DeletedByAccountId ?? Guid.Empty;

            foreach (var schedule in schedules)
            {
                // Chỉ hủy các ca chưa Completed / Cancelled
                if (schedule.Status == DoctorScheduleStatus.Completed
                    || schedule.Status == DoctorScheduleStatus.Cancelled)
                    continue;

                schedule.Cancel(
                    cancelledByAccountId: cancelledBy,
                    reason: cancelReason);
            }

            // ==================== BƯỚC 4: Xóa mềm doctor ====================
            _doctorRepo.Delete(doctor);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}