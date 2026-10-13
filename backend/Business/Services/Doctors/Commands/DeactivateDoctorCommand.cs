using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Doctor;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Tạm ngưng bác sĩ.
    /// 
    /// CẢNH BÁO (nhưng vẫn cho deactivate):
    /// - Nếu có Appointment chưa Completed trong tương lai.
    /// </summary>
    public record DeactivateDoctorCommand(Guid Id)
        : IRequest<DeactivateDoctorResultDto>;

    public class DeactivateDoctorCommandHandler
        : IRequestHandler<DeactivateDoctorCommand, DeactivateDoctorResultDto>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateDoctorCommandHandler(
            IDoctorRepository doctorRepo,
            IAppointmentRepository appointmentRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _appointmentRepo = appointmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeactivateDoctorResultDto> Handle(
            DeactivateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            if (!doctor.IsActive)
                throw new DomainException("Bác sĩ này đã ở trạng thái tạm ngưng.");

            // Cảnh báo nếu có appointment chưa completed trong tương lai
            var warnings = new List<string>();
            var today = DateTime.UtcNow.Date;

            var pendingCount = await _appointmentRepo.GetQueryable()
                .CountAsync(a => a.RequestedDoctorId == doctor.Id
                              && a.Status != AppointmentStatus.Completed
                              && a.Status != AppointmentStatus.Cancelled
                              && a.Status != AppointmentStatus.NoShow
                              && a.AppointmentDate.Date >= today,
                          cancellationToken);

            if (pendingCount > 0)
            {
                warnings.Add(
                    $"Bác sĩ có {pendingCount} lịch hẹn chưa hoàn thành trong tương lai. " +
                    "Vui lòng xử lý các lịch hẹn này.");
            }

            doctor.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DeactivateDoctorResultDto(
                Succeeded: true,
                Warnings: warnings);
        }
    }
}