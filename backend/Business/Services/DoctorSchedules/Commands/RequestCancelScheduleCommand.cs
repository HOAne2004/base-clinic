using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Gửi yêu cầu hủy ca (dùng khi ca < 3 ngày).
    /// 
    /// VALIDATION:
    /// - Ca tồn tại + chưa Cancelled/Completed.
    /// - Ca còn < 3 ngày (nếu ≥ 3 ngày → dùng CancelScheduleCommand).
    /// - Chưa có yêu cầu Pending cho ca này.
    /// </summary>
    public record RequestCancelScheduleCommand(
        Guid ScheduleId,
        Guid RequesterAccountId,
        string Reason
    ) : IRequest<Guid>;

    public class RequestCancelScheduleCommandHandler
        : IRequestHandler<RequestCancelScheduleCommand, Guid>
    {
        private const int MinDaysForDirectCancel = 3;

        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IRepository<ScheduleCancellationRequest> _requestRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RequestCancelScheduleCommandHandler(
            IDoctorScheduleRepository scheduleRepo,
            IRepository<ScheduleCancellationRequest> requestRepo,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepo = scheduleRepo;
            _requestRepo = requestRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            RequestCancelScheduleCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Validate input ====================
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new DomainException("Lý do hủy không được để trống.");

            // ==================== BƯỚC 2: Load schedule ====================
            var schedule = await _scheduleRepo.GetByIdAsync(
                request.ScheduleId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            // ==================== BƯỚC 3: Validate status ====================
            if (schedule.Status == DoctorScheduleStatus.Cancelled)
                throw new DomainException("Ca này đã được hủy trước đó.");

            if (schedule.Status == DoctorScheduleStatus.Completed)
                throw new DomainException("Không thể hủy ca đã hoàn thành.");

            // ==================== BƯỚC 4: Validate thời gian ====================
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var daysUntil = schedule.WorkDate.DayNumber - today.DayNumber;

            if (daysUntil >= MinDaysForDirectCancel)
                throw new DomainException(
                    $"Ca khám còn {daysUntil} ngày (từ {MinDaysForDirectCancel} ngày trở lên). " +
                    $"Vui lòng sử dụng chức năng hủy trực tiếp thay vì gửi yêu cầu.");

            if (daysUntil < 0)
                throw new DomainException("Không thể hủy ca đã qua.");

            // ==================== BƯỚC 5: Check đã có yêu cầu Pending chưa ====================
            var existingPending = await _requestRepo.GetQueryableWithTracking()
                .AnyAsync(r => r.DoctorScheduleId == request.ScheduleId
                            && r.Status == ScheduleCancellationRequestStatus.Pending,
                          cancellationToken);

            if (existingPending)
                throw new DomainException(
                    "Đã có yêu cầu hủy đang chờ duyệt cho ca này.");

            // ==================== BƯỚC 6: Tạo request ====================
            var cancellationRequest = ScheduleCancellationRequest.Create(
                doctorScheduleId: request.ScheduleId,
                requesterAccountId: request.RequesterAccountId,
                reason: request.Reason);

            _requestRepo.Add(cancellationRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return cancellationRequest.Id;
        }
    }
}