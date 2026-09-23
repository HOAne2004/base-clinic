using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Domain.Entities
{
    public class Consultation : AggregateRoot
    {
        public Guid DoctorId { get; private set; }
        public Guid EncounterId { get; private set; }
        public DateTime? StartAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public ConsultationStatus Status { get; private set; }

        public string? DiagnosisCode { get; private set; }
        public string? DiagnosisName { get; private set; }
        public string? TreatmentPlan { get; private set; }

        public DateTime? FollowUpDate { get; private set; }
        public DateTime? RecordDate { get; private set; }

        private Consultation() { } // Dành cho EF Core

        public Consultation(Guid doctorId, Guid encounterId)
        {
            DoctorId = doctorId;
            EncounterId = encounterId;
            Status = ConsultationStatus.InProgress;
            StartAt = DateTime.UtcNow;
            RecordDate = DateTime.UtcNow;
        }

        public void Complete(DateTime completedAt, string diagnosisCode, string diagnosisName, string treatmentPlan, DateTime? followUpDate)
        {
            if (Status != ConsultationStatus.InProgress)
                throw new InvalidOperationException("Chỉ có thể hoàn thành phiên khám đang diễn ra (InProgress).");

            if (StartAt.HasValue && completedAt < StartAt.Value)
                throw new ArgumentException("Thời gian hoàn thành không được diễn ra trước thời gian bắt đầu.");

            if (string.IsNullOrWhiteSpace(diagnosisName))
                throw new ArgumentException("Chẩn đoán bệnh không được để trống.");

            if (string.IsNullOrWhiteSpace(treatmentPlan))
                throw new ArgumentException("Hướng điều trị không được để trống.");

            CompletedAt = completedAt;
            DiagnosisCode = diagnosisCode?.Trim();
            DiagnosisName = diagnosisName.Trim();
            TreatmentPlan = treatmentPlan.Trim();
            FollowUpDate = followUpDate;
            Status = ConsultationStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == ConsultationStatus.Completed)
                throw new InvalidOperationException("Không thể hủy phiên khám đã hoàn tất. Vui lòng liên hệ quản trị viên nếu có sai sót.");

            Status = ConsultationStatus.Cancelled;
        }

        public void UpdateFollowUpDate(DateTime followUpDate)
        {
            if (Status == ConsultationStatus.Cancelled)
                throw new InvalidOperationException("Không thể đặt lịch tái khám cho phiên khám đã hủy.");
            FollowUpDate = followUpDate;
        }
    }
}