using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Hủy ca làm việc trực tiếp.
    /// 
    /// RULE:
    /// - Ca >= 3 ngày: Doctor/Receptionist/Admin đều hủy được.
    /// - Ca < 3 ngày: CHỈ Admin hủy được (Doctor/Receptionist phải qua workflow).
    /// 
    /// Admin có thể truyền IsAdmin = true để bypass rule 3 ngày.
    /// </summary>
    public record CancelScheduleCommand(
        Guid ScheduleId,
        Guid CancelledByAccountId,
        string Reason,
        bool IsAdmin = false
    ) : IRequest<CancelScheduleResultDto>;

    public class CancelScheduleCommandHandler
        : IRequestHandler<CancelScheduleCommand, CancelScheduleResultDto>
    {
        /// <summary>Ngưỡng tối thiểu (ngày) cho phép hủy trực tiếp.</summary>
        private const int MinDaysForDirectCancel = 3;

        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IScheduleCancellationService _cancellationService;
        private readonly IUnitOfWork _unitOfWork;

        public CancelScheduleCommandHandler(
            IDoctorScheduleRepository scheduleRepo,
            IScheduleCancellationService cancellationService,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepo = scheduleRepo;
            _cancellationService = cancellationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CancelScheduleResultDto> Handle(
            CancelScheduleCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Validate input ====================
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new DomainException("Lý do hủy không được để trống.");

            if (request.Reason.Length > 500)
                throw new DomainException("Lý do hủy không vượt quá 500 ký tự.");

            // ==================== BƯỚC 2: Load schedule ====================
            var schedule = await _scheduleRepo.GetByIdAsync(
                request.ScheduleId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            // ==================== BƯỚC 3: Check rule 3 ngày ====================
            if (!request.IsAdmin)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysUntil = schedule.WorkDate.DayNumber - today.DayNumber;

                if (daysUntil < MinDaysForDirectCancel)
                {
                    throw new DomainException(
                        $"Ca khám còn {daysUntil} ngày (dưới {MinDaysForDirectCancel} ngày). " +
                        $"Chỉ Admin mới có thể hủy trực tiếp. " +
                        $"Vui lòng gửi yêu cầu hủy để Admin duyệt.");
                }
            }

            // ==================== BƯỚC 4: Hủy ca (service xử lý cascade) ====================
            var result = await _cancellationService.CancelAsync(
                scheduleId: request.ScheduleId,
                cancelledByAccountId: request.CancelledByAccountId,
                reason: request.Reason,
                cancellationToken: cancellationToken);

            // ==================== BƯỚC 5: Save ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}