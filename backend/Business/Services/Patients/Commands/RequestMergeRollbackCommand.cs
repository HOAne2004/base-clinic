using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Gửi yêu cầu rollback merge.
    /// 
    /// CHẶN nếu:
    /// - Merge log không tồn tại.
    /// - Merge log đã rollback.
    /// - Đã có yêu cầu Pending cho cùng merge log.
    /// - Ngoài time window (> 7 ngày).
    /// </summary>
    public record RequestMergeRollbackCommand(
        Guid PatientMergeLogId,
        Guid RequesterAccountId,
        string Reason
    ) : IRequest<Guid>;

    public class RequestMergeRollbackCommandHandler
        : IRequestHandler<RequestMergeRollbackCommand, Guid>
    {
        /// <summary>Time window tối đa cho phép rollback (7 ngày).</summary>
        private const int MaxRollbackDays = 7;

        private readonly IRepository<PatientMergeLog> _mergeLogRepo;
        private readonly IRepository<MergeRollbackRequest> _requestRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RequestMergeRollbackCommandHandler(
            IRepository<PatientMergeLog> mergeLogRepo,
            IRepository<MergeRollbackRequest> requestRepo,
            IUnitOfWork unitOfWork)
        {
            _mergeLogRepo = mergeLogRepo;
            _requestRepo = requestRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            RequestMergeRollbackCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm merge log ====================
            var mergeLog = await _mergeLogRepo.GetByIdAsync(
                request.PatientMergeLogId, cancellationToken);

            if (mergeLog == null)
                throw new NotFoundException("Không tìm thấy merge log.");

            // ==================== BƯỚC 2: Check đã rollback chưa ====================
            if (mergeLog.IsRolledBack)
                throw new DomainException(
                    "Merge log này đã được rollback trước đó.");

            // ==================== BƯỚC 3: Check time window ====================
            var elapsed = DateTimeOffset.UtcNow - mergeLog.MergedAt;
            if (elapsed.TotalDays > MaxRollbackDays)
                throw new DomainException(
                    $"Đã quá {MaxRollbackDays} ngày kể từ khi merge. " +
                    $"Không thể rollback.");

            // ==================== BƯỚC 4: Check pending request ====================
            var existingPending = await _requestRepo
                .GetQueryableWithTracking()
                .AnyAsync(r => r.PatientMergeLogId == request.PatientMergeLogId
                            && r.Status == MergeRollbackRequestStatus.Pending,
                          cancellationToken);

            if (existingPending)
                throw new DomainException(
                    "Đã có yêu cầu rollback đang chờ duyệt cho merge log này.");

            // ==================== BƯỚC 5: Tạo yêu cầu ====================
            var rollbackRequest = MergeRollbackRequest.Create(
                patientMergeLogId: request.PatientMergeLogId,
                requesterAccountId: request.RequesterAccountId,
                reason: request.Reason);

            _requestRepo.Add(rollbackRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return rollbackRequest.Id;
        }
    }
}