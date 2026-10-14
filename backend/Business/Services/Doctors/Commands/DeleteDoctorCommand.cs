using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
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
    /// - DoctorSchedule: tự động hủy hết.
    /// </summary>
    public record DeleteDoctorCommand(Guid Id) : IRequest<bool>;

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
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            // Check không có Appointment
            var hasAppointments = await _appointmentRepo.GetQueryable()
                .AnyAsync(a => a.RequestedDoctorId == doctor.Id, cancellationToken);

            if (hasAppointments)
                throw new DomainException(
                    "Không thể xóa bác sĩ vì vẫn còn lịch hẹn. " +
                    "Vui lòng xử lý các lịch hẹn trước.");

            // Hủy toàn bộ DoctorSchedule chưa Completed
            var schedules = await _scheduleRepo.GetQueryableWithTracking()
                .Where(s => s.DoctorId == doctor.Id)
                .ToListAsync(cancellationToken);

            foreach (var schedule in schedules)
            {
                if (schedule.Status != Domain.Enums.DoctorScheduleStatus.Completed
                    && schedule.Status != Domain.Enums.DoctorScheduleStatus.Cancelled)
                {
                    schedule.Cancel(schedule.Id, schedule.CancelReason);
                }
            }

            _doctorRepo.Delete(doctor);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}