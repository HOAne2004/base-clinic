using BaseClinic.Domain.Common;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Log merge Patient.
    /// Append-only — không sửa, chỉ thêm.
    /// Cho phép audit + rollback.
    /// </summary>
    public class PatientMergeLog : Entity
    {
        /// <summary>Patient được giữ lại (survivor).</summary>
        public Guid SurvivorPatientId { get; private set; }
        public Patient SurvivorPatient { get; private set; } = null!;

        /// <summary>Patient bị gộp (merged).</summary>
        public Guid MergedPatientId { get; private set; }

        /// <summary>Snapshot dữ liệu của merged patient (JSON) — cho rollback.</summary>
        public string MergedSnapshot { get; private set; } = string.Empty;

        /// <summary>Lý do merge.</summary>
        public string Reason { get; private set; } = string.Empty;

        /// <summary>Ai merge.</summary>
        public Guid MergedByAccountId { get; private set; }

        public DateTimeOffset MergedAt { get; private set; }

        // ==================== ROLLBACK ====================
        public bool IsRolledBack { get; private set; }
        public DateTimeOffset? RolledBackAt { get; private set; }
        public Guid? RolledBackByAccountId { get; private set; }
        public string? RollbackReason { get; private set; }

        private PatientMergeLog() { } // EF Core

        public PatientMergeLog(
            Guid survivorPatientId,
            Guid mergedPatientId,
            string mergedSnapshot,
            string reason,
            Guid mergedByAccountId)
        {
            if (survivorPatientId == Guid.Empty)
                throw new DomainException("SurvivorPatientId không hợp lệ.");
            if (mergedPatientId == Guid.Empty)
                throw new DomainException("MergedPatientId không hợp lệ.");
            if (survivorPatientId == mergedPatientId)
                throw new DomainException("Không thể merge Patient vào chính nó.");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Lý do merge không được để trống.");

            SurvivorPatientId = survivorPatientId;
            MergedPatientId = mergedPatientId;
            MergedSnapshot = mergedSnapshot;
            Reason = reason.Trim();
            MergedByAccountId = mergedByAccountId;
            MergedAt = DateTimeOffset.UtcNow;
            IsRolledBack = false;
        }

        public void MarkAsRolledBack(Guid rolledBackBy, string reason)
        {
            if (IsRolledBack)
                throw new DomainException("Merge log này đã được rollback trước đó.");

            IsRolledBack = true;
            RolledBackAt = DateTimeOffset.UtcNow;
            RolledBackByAccountId = rolledBackBy;
            RollbackReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        }
    }
}