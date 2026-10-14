using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Yêu cầu hủy ca làm việc — dùng khi ca còn < 3 ngày.
    /// 
    /// KHÁC BIỆT với hủy trực tiếp:
    /// - Hủy trực tiếp: ca >= 3 ngày, Doctor/Receptionist/Admin gọi API cancel.
    /// - Yêu cầu hủy: ca < 3 ngày, Doctor/Receptionist gửi yêu cầu, Admin duyệt.
    /// 
    /// KHÔNG cần soft delete — chỉ cần audit qua CreatedDate + ReviewedAt.
    /// </summary>
    public class ScheduleCancellationRequest : Entity
    {
        // ==================== LIÊN KẾT ====================
        /// <summary>Ca làm việc cần hủy.</summary>
        public Guid DoctorScheduleId { get; private set; }
        public DoctorSchedule DoctorSchedule { get; private set; } = null!;

        // ==================== REQUESTER ====================
        /// <summary>Ai gửi yêu cầu.</summary>
        public Guid RequestedByAccountId { get; private set; }
        public Account RequestedByAccount { get; private set; } = null!;

        /// <summary>Lý do yêu cầu hủy.</summary>
        public string Reason { get; private set; } = string.Empty;

        // ==================== STATUS ====================
        public ScheduleCancellationRequestStatus Status { get; private set; }

        // ==================== REVIEW ====================
        /// <summary>Ai duyệt (Admin).</summary>
        public Guid? ReviewedByAccountId { get; private set; }
        public Account? ReviewedByAccount { get; private set; }

        public DateTimeOffset? ReviewedAt { get; private set; }

        /// <summary>Ghi chú của người duyệt.</summary>
        public string? ReviewNote { get; private set; }

        // ==================== AUDIT ====================
        public DateTimeOffset CreatedDate { get; private set; }

        // ==================== CONSTRUCTOR ====================
        private ScheduleCancellationRequest() { } // EF Core

        /// <summary>
        /// Factory: tạo yêu cầu hủy mới.
        /// </summary>
        public static ScheduleCancellationRequest Create(
            Guid doctorScheduleId,
            Guid requesterAccountId,
            string reason)
        {
            if (doctorScheduleId == Guid.Empty)
                throw new DomainException("DoctorScheduleId không hợp lệ.");
            if (requesterAccountId == Guid.Empty)
                throw new DomainException("RequesterAccountId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Lý do yêu cầu không được để trống.");
            if (reason.Length > 500)
                throw new DomainException("Lý do không vượt quá 500 ký tự.");

            return new ScheduleCancellationRequest
            {
                DoctorScheduleId = doctorScheduleId,
                RequestedByAccountId = requesterAccountId,
                Reason = reason.Trim(),
                Status = ScheduleCancellationRequestStatus.Pending,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        // ==================== BUSINESS METHODS ====================

        /// <summary>
        /// Admin duyệt yêu cầu. Chỉ khi Status = Pending.
        /// LƯU Ý: Việc hủy ca được thực hiện ở tầng Service.
        /// </summary>
        public void Approve(Guid reviewerAccountId, string? reviewNote)
        {
            if (Status != ScheduleCancellationRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể duyệt yêu cầu đang ở trạng thái Pending.");
            if (reviewerAccountId == Guid.Empty)
                throw new DomainException("ReviewerAccountId không hợp lệ.");

            Status = ScheduleCancellationRequestStatus.Approved;
            ReviewedByAccountId = reviewerAccountId;
            ReviewedAt = DateTimeOffset.UtcNow;
            ReviewNote = string.IsNullOrWhiteSpace(reviewNote)
                ? null
                : reviewNote.Trim();
        }

        /// <summary>
        /// Admin từ chối yêu cầu. Chỉ khi Status = Pending.
        /// </summary>
        public void Reject(Guid reviewerAccountId, string reviewNote)
        {
            if (Status != ScheduleCancellationRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể từ chối yêu cầu đang ở trạng thái Pending.");
            if (reviewerAccountId == Guid.Empty)
                throw new DomainException("ReviewerAccountId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(reviewNote))
                throw new DomainException("Lý do từ chối không được để trống.");

            Status = ScheduleCancellationRequestStatus.Rejected;
            ReviewedByAccountId = reviewerAccountId;
            ReviewedAt = DateTimeOffset.UtcNow;
            ReviewNote = reviewNote.Trim();
        }

        /// <summary>
        /// Requester tự hủy yêu cầu. Chỉ khi Status = Pending.
        /// </summary>
        public void CancelByRequester(Guid requesterAccountId)
        {
            if (Status != ScheduleCancellationRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể hủy yêu cầu đang ở trạng thái Pending.");
            if (RequestedByAccountId != requesterAccountId)
                throw new DomainException(
                    "Chỉ người gửi yêu cầu mới có thể hủy.");

            Status = ScheduleCancellationRequestStatus.Cancelled;
        }
    }
}