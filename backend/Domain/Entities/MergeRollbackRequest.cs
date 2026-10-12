using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Yêu cầu rollback merge Patient.
    /// 
    /// Ai được gửi:
    /// - Patient (tự gửi qua /me).
    /// - Lễ tân (gửi hộ).
    /// - Admin / SuperAdmin (gửi trực tiếp hoặc tự approve).
    /// 
    /// Ai được duyệt:
    /// - Admin (trong 24h).
    /// - SuperAdmin (trong 7 ngày).
    /// 
    /// KHÔNG cần soft delete — chỉ cần audit qua CreatedDate + ReviewedAt.
    /// </summary>
    public class MergeRollbackRequest : Entity
    {
        // ==================== LIÊN KẾT ====================
        /// <summary>Merge log mà yêu cầu nhắm đến.</summary>
        public Guid PatientMergeLogId { get; private set; }
        public PatientMergeLog PatientMergeLog { get; private set; } = null!;

        // ==================== REQUESTER ====================
        /// <summary>Ai gửi yêu cầu.</summary>
        public Guid RequestedByAccountId { get; private set; }
        public Account RequestedByAccount { get; private set; } = null!;

        /// <summary>Lý do yêu cầu rollback.</summary>
        public string Reason { get; private set; } = string.Empty;

        // ==================== STATUS ====================
        public MergeRollbackRequestStatus Status { get; private set; }

        // ==================== REVIEW ====================
        /// <summary>Ai duyệt (Admin/SuperAdmin).</summary>
        public Guid? ReviewedByAccountId { get; private set; }
        public Account? ReviewedByAccount { get; private set; }

        public DateTimeOffset? ReviewedAt { get; private set; }

        /// <summary>Ghi chú của người duyệt.</summary>
        public string? ReviewNote { get; private set; }

        /// <summary>
        /// JSON lưu kết quả resolve conflict.
        /// Format: [{ recordType, recordId, belongsTo }]
        /// </summary>
        public string? ConflictResolutions { get; private set; }

        // ==================== AUDIT ====================
        public DateTimeOffset CreatedDate { get; private set; }

        // ==================== CONSTRUCTOR ====================
        private MergeRollbackRequest() { } // EF Core

        /// <summary>
        /// Factory: tạo yêu cầu rollback mới.
        /// </summary>
        public static MergeRollbackRequest Create(
            Guid patientMergeLogId,
            Guid requesterAccountId,
            string reason)
        {
            if (patientMergeLogId == Guid.Empty)
                throw new DomainException("PatientMergeLogId không hợp lệ.");
            if (requesterAccountId == Guid.Empty)
                throw new DomainException("RequesterAccountId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Lý do yêu cầu không được để trống.");
            if (reason.Length > 500)
                throw new DomainException("Lý do không vượt quá 500 ký tự.");

            return new MergeRollbackRequest
            {
                PatientMergeLogId = patientMergeLogId,
                RequestedByAccountId = requesterAccountId,
                Reason = reason.Trim(),
                Status = MergeRollbackRequestStatus.Pending,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        // ==================== BUSINESS METHODS ====================

        /// <summary>
        /// Duyệt yêu cầu. Chỉ khi Status = Pending.
        /// </summary>
        public void Approve(
            Guid reviewerAccountId,
            string? reviewNote,
            string? conflictResolutions)
        {
            if (Status != MergeRollbackRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể duyệt yêu cầu đang ở trạng thái Pending.");
            if (reviewerAccountId == Guid.Empty)
                throw new DomainException("ReviewerAccountId không hợp lệ.");

            Status = MergeRollbackRequestStatus.Approved;
            ReviewedByAccountId = reviewerAccountId;
            ReviewedAt = DateTimeOffset.UtcNow;
            ReviewNote = string.IsNullOrWhiteSpace(reviewNote)
                ? null
                : reviewNote.Trim();
            ConflictResolutions = conflictResolutions;
        }

        /// <summary>
        /// Từ chối yêu cầu. Chỉ khi Status = Pending.
        /// </summary>
        public void Reject(Guid reviewerAccountId, string reviewNote)
        {
            if (Status != MergeRollbackRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể từ chối yêu cầu đang ở trạng thái Pending.");
            if (reviewerAccountId == Guid.Empty)
                throw new DomainException("ReviewerAccountId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(reviewNote))
                throw new DomainException("Lý do từ chối không được để trống.");

            Status = MergeRollbackRequestStatus.Rejected;
            ReviewedByAccountId = reviewerAccountId;
            ReviewedAt = DateTimeOffset.UtcNow;
            ReviewNote = reviewNote.Trim();
        }

        /// <summary>
        /// Requester tự hủy. Chỉ khi Status = Pending.
        /// </summary>
        public void Cancel(Guid requesterAccountId)
        {
            if (Status != MergeRollbackRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể hủy yêu cầu đang ở trạng thái Pending.");
            if (RequestedByAccountId != requesterAccountId)
                throw new DomainException(
                    "Chỉ người gửi yêu cầu mới có thể hủy.");

            Status = MergeRollbackRequestStatus.Cancelled;
        }
    }
}