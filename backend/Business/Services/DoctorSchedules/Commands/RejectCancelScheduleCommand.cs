using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Admin từ chối yêu cầu hủy ca.
    /// </summary>
    public record RejectCancelScheduleCommand(
        Guid RequestId,
        Guid ReviewerAccountId,
        string ReviewNote
    ) : IRequest<bool>;

    public class RejectCancelScheduleCommandHandler
        : IRequestHandler<RejectCancelScheduleCommand, bool>
    {
        private readonly IRepository<ScheduleCancellationRequest> _requestRepo;
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;

        public RejectCancelScheduleCommandHandler(
            IRepository<ScheduleCancellationRequest> requestRepo,
            IDoctorScheduleRepository scheduleRepo,
            IAccountRepository accountRepo,
            ISmsService smsService,
            IUnitOfWork unitOfWork)
        {
            _requestRepo = requestRepo;
            _scheduleRepo = scheduleRepo;
            _accountRepo = accountRepo;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            RejectCancelScheduleCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Load request ====================
            var cancelRequest = await _requestRepo.GetByIdAsync(
                request.RequestId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy yêu cầu hủy.");

            // ==================== BƯỚC 2: Mark Rejected ====================
            cancelRequest.Reject(request.ReviewerAccountId, request.ReviewNote);

            // ==================== BƯỚC 3: SMS cho requester ====================
            var schedule = await _scheduleRepo.GetByIdAsync(
                cancelRequest.DoctorScheduleId, cancellationToken);

            var requester = await _accountRepo.GetByIdAsync(
                cancelRequest.RequestedByAccountId, cancellationToken);

            if (schedule != null && requester != null
                && !string.IsNullOrWhiteSpace(requester.PhoneNumber))
            {
                var dateStr = schedule.WorkDate.ToString("dd/MM/yyyy");
                var timeStr = $"{schedule.StartTime:HH\\:mm}-{schedule.EndTime:HH\\:mm}";

                await _smsService.SendNotificationAsync(
                    phoneNumber: requester.PhoneNumber,
                    message: $"[BaseClinic] Yêu cầu hủy ca ngày {dateStr} ({timeStr}) " +
                             $"đã bị từ chối. Lý do: {request.ReviewNote}",
                    cancellationToken: cancellationToken);
            }

            // ==================== BƯỚC 4: Save ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}