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
    /// Admin duyệt yêu cầu hủy ca.
    /// 
    /// LUỒNG:
    /// 1. Load request + schedule.
    /// 2. Validate request Pending + schedule chưa Cancelled/Completed.
    /// 3. Cascade hủy ca (dùng ScheduleCancellationService).
    /// 4. Mark request = Approved.
    /// 5. SMS cho requester.
    /// 6. SaveChanges.
    /// </summary>
    public record ApproveCancelScheduleCommand(
        Guid RequestId,
        Guid ReviewerAccountId,
        string? ReviewNote
    ) : IRequest<CancelScheduleResultDto>;

    public class ApproveCancelScheduleCommandHandler
        : IRequestHandler<ApproveCancelScheduleCommand, CancelScheduleResultDto>
    {
        private readonly IRepository<ScheduleCancellationRequest> _requestRepo;
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IScheduleCancellationService _cancellationService;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveCancelScheduleCommandHandler(
            IRepository<ScheduleCancellationRequest> requestRepo,
            IDoctorScheduleRepository scheduleRepo,
            IAccountRepository accountRepo,
            IScheduleCancellationService cancellationService,
            ISmsService smsService,
            IUnitOfWork unitOfWork)
        {
            _requestRepo = requestRepo;
            _scheduleRepo = scheduleRepo;
            _accountRepo = accountRepo;
            _cancellationService = cancellationService;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CancelScheduleResultDto> Handle(
            ApproveCancelScheduleCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Load request ====================
            var cancelRequest = await _requestRepo.GetByIdAsync(
                request.RequestId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy yêu cầu hủy.");

            if (cancelRequest.Status != ScheduleCancellationRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể duyệt yêu cầu đang ở trạng thái Pending.");

            // ==================== BƯỚC 2: Load schedule ====================
            var schedule = await _scheduleRepo.GetByIdWithItemsAsync(
                cancelRequest.DoctorScheduleId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            // ==================== BƯỚC 3: Cascade hủy ca ====================
            var cancelResult = await _cancellationService.CancelAsync(
                scheduleId: schedule.Id,
                cancelledByAccountId: request.ReviewerAccountId,
                reason: cancelRequest.Reason,
                cancellationToken: cancellationToken);

            // ==================== BƯỚC 4: Mark request = Approved ====================
            cancelRequest.Approve(request.ReviewerAccountId, request.ReviewNote);

            // ==================== BƯỚC 5: SMS cho requester ====================
            await SendReviewNotificationAsync(
                cancelRequest,
                schedule,
                isApproved: true,
                cancellationToken);

            // ==================== BƯỚC 6: Save ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return cancelResult;
        }

        private async Task SendReviewNotificationAsync(
            ScheduleCancellationRequest cancelRequest,
            Domain.Entities.DoctorSchedule schedule,
            bool isApproved,
            CancellationToken cancellationToken)
        {
            var requester = await _accountRepo.GetByIdAsync(
                cancelRequest.RequestedByAccountId, cancellationToken);

            if (requester == null || string.IsNullOrWhiteSpace(requester.PhoneNumber))
                return;

            var dateStr = schedule.WorkDate.ToString("dd/MM/yyyy");
            var timeStr = $"{schedule.StartTime:HH\\:mm}-{schedule.EndTime:HH\\:mm}";

            var message = isApproved
                ? $"[BaseClinic] Yêu cầu hủy ca ngày {dateStr} ({timeStr}) đã được duyệt."
                : $"[BaseClinic] Yêu cầu hủy ca ngày {dateStr} ({timeStr}) đã bị từ chối. " +
                  $"Lý do: {cancelRequest.ReviewNote ?? "Không có"}";

            await _smsService.SendNotificationAsync(
                phoneNumber: requester.PhoneNumber,
                message: message,
                cancellationToken: cancellationToken);
        }
    }
}