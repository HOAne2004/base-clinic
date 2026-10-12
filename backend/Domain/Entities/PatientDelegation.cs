using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Ủy quyền theo dõi hồ sơ bệnh nhân.
    /// 
    /// LUỒNG:
    /// 1. A tạo Patient B → auto-grant Full cho A (owner delegation).
    /// 2. C muốn theo dõi B → gửi request → chờ approve.
    ///    - Nếu B chưa có Account: A (owner) approve.
    ///    - Nếu B đã có Account: B approve.
    /// 3. B claim Account → B có thể revoke delegation của A/C.
    /// </summary>
    public class PatientDelegation : AuditableEntity
    {
        // ==================== LIÊN KẾT ====================
        public Guid TargetPatientId { get; private set; }
        public Patient TargetPatient { get; private set; } = null!;

        public Guid ObserverAccountId { get; private set; }
        public Account ObserverAccount { get; private set; } = null!;

        // ==================== LOẠI QUAN HỆ ====================
        public PatientRelationshipType RelationshipType { get; private set; }

        /// <summary>
        /// Mức độ truy cập:
        /// - Limited: Chỉ thấy lịch sử do chính Observer tạo.
        /// - Full: Thấy mọi lịch sử.
        /// </summary>
        public DelegationAccessLevel AccessLevel { get; private set; }

        /// <summary>Trạng thái: Pending / Accepted / Rejected / Revoked.</summary>
        public DelegationStatus Status { get; private set; }

        // ==================== AUDIT NGHIỆP VỤ ====================
        public Guid? RequestedByAccountId { get; private set; }
        public Guid? ApprovedByAccountId { get; private set; }
        public DateTimeOffset? GrantedAt { get; private set; }
        public DateTimeOffset? RevokedAt { get; private set; }
        public string? RevokeReason { get; private set; }

        // ==================== CONSTRUCTOR ====================
        private PatientDelegation() { } // EF Core

        /// <summary>
        /// Factory: auto-grant khi A tạo Patient B — A có Full quyền.
        /// </summary>
        public static PatientDelegation CreateOwnerDelegation(
            Guid targetPatientId,
            Guid ownerAccountId,
            PatientRelationshipType relationshipType)
        {
            if (targetPatientId == Guid.Empty)
                throw new DomainException("TargetPatientId không hợp lệ.");
            if (ownerAccountId == Guid.Empty)
                throw new DomainException("OwnerAccountId không hợp lệ.");

            return new PatientDelegation
            {
                TargetPatientId = targetPatientId,
                ObserverAccountId = ownerAccountId,
                RelationshipType = relationshipType,
                AccessLevel = DelegationAccessLevel.Full,
                Status = DelegationStatus.Accepted,
                GrantedAt = DateTimeOffset.UtcNow
            };
        }

        /// <summary>
        /// Factory: request từ bên thứ 2 — chờ approve.
        /// </summary>
        public static PatientDelegation CreateRequest(
            Guid targetPatientId,
            Guid requesterAccountId,
            PatientRelationshipType relationshipType,
            DelegationAccessLevel accessLevel = DelegationAccessLevel.Limited)
        {
            if (targetPatientId == Guid.Empty)
                throw new DomainException("TargetPatientId không hợp lệ.");
            if (requesterAccountId == Guid.Empty)
                throw new DomainException("RequesterAccountId không hợp lệ.");

            return new PatientDelegation
            {
                TargetPatientId = targetPatientId,
                ObserverAccountId = requesterAccountId,
                RelationshipType = relationshipType,
                AccessLevel = accessLevel,
                Status = DelegationStatus.Pending,
                RequestedByAccountId = requesterAccountId
            };
        }

        // ==================== BUSINESS METHODS ====================

        internal void SetTargetPatientIdInternal(Guid patientId)
        {
            TargetPatientId = patientId;
        }
        public void Approve(Guid approverAccountId)
        {
            if (Status != DelegationStatus.Pending)
                throw new DomainException("Chỉ có thể approve delegation đang ở trạng thái Pending.");
            if (approverAccountId == Guid.Empty)
                throw new DomainException("ApproverAccountId không hợp lệ.");

            Status = DelegationStatus.Accepted;
            ApprovedByAccountId = approverAccountId;
            GrantedAt = DateTimeOffset.UtcNow;
        }

        public void Reject(Guid rejecterAccountId, string? reason = null)
        {
            if (Status != DelegationStatus.Pending)
                throw new DomainException("Chỉ có thể reject delegation đang ở trạng thái Pending.");
            if (rejecterAccountId == Guid.Empty)
                throw new DomainException("RejecterAccountId không hợp lệ.");

            Status = DelegationStatus.Rejected;
            ApprovedByAccountId = rejecterAccountId;
            RevokeReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        }

        public void Revoke(string reason)
        {
            if (Status != DelegationStatus.Accepted)
                throw new DomainException("Chỉ có thể revoke delegation đang Accepted.");

            Status = DelegationStatus.Revoked;
            RevokedAt = DateTimeOffset.UtcNow;
            RevokeReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        }

        public void ChangeAccessLevel(DelegationAccessLevel newLevel)
        {
            if (Status != DelegationStatus.Accepted)
                throw new DomainException("Chỉ có thể đổi AccessLevel khi delegation đang Accepted.");

            AccessLevel = newLevel;
        }

        public bool CanViewOwnCreatedHistory() => Status == DelegationStatus.Accepted;

        public bool CanViewAllHistory()
            => Status == DelegationStatus.Accepted
               && AccessLevel == DelegationAccessLevel.Full;
    }
}