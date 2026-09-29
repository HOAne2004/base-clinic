using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using System;

namespace BaseClinic.Domain.Entities
{
    public class PatientDelegation : AuditableEntity
    {
        // Hồ sơ bệnh nhân đang bị theo dõi
        public Guid TargetPatientId { get; private set; }

        // Tài khoản được cấp quyền xem hồ sơ
        public Guid ObserverAccountId { get; private set; }

        public PatientRelationshipType RelationshipType { get; private set; }

        // Enum trạng thái: Pending (Chờ xác nhận), Accepted (Đã đồng ý), Revoked (Đã thu hồi)
        public DelegationStatus Status { get; private set; }

        private PatientDelegation() { }

        public PatientDelegation(Guid targetPatientId, Guid observerAccountId, PatientRelationshipType relationshipType, DelegationStatus status)
        {
            if (targetPatientId == Guid.Empty) throw new ArgumentException("Mã tài khoản cấp phép không hợp lệ.");
            if (observerAccountId == Guid.Empty) throw new ArgumentException("Mã tài khoản được cấp phép không hợp lệ.");

            TargetPatientId = targetPatientId;
            ObserverAccountId = observerAccountId;
            RelationshipType = relationshipType;
            Status = status;
        }

        public void ChangeStatus(DelegationStatus newStatus)
        {
            Status = newStatus;
        }
    }
}