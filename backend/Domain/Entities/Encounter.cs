using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using System;

namespace BaseClinic.Domain.Entities
{
    public class Encounter : AggregateRoot
    {
        public string EncounterCode { get; private set; } = null!;
        public Guid PatientId { get; private set; }
        public Guid AppointmentId { get; private set; }
        public DateTime CheckInAt { get; private set; }
        public DateTime? EndAt { get; private set; }
        public EncounterStatus Status { get; private set; }

        private Encounter() { }

        public Encounter(string encounterCode, Guid patientId, Guid appointmentId, DateTime checkInAt)
        {
            if (string.IsNullOrWhiteSpace(encounterCode))
                throw new ArgumentException("Mã lượt khám không được để trống.");

            EncounterCode = encounterCode;
            PatientId = patientId;
            AppointmentId = appointmentId;
            CheckInAt = checkInAt;
            Status = EncounterStatus.Waiting;
        }

        public void StartConsultation()
        {
            if (Status != EncounterStatus.Waiting)
                throw new InvalidOperationException("Chỉ có thể bắt đầu khám khi bệnh nhân đang chờ (Waiting).");
            Status = EncounterStatus.InProgress;
        }

        public void CompleteEncounter(DateTime endAt)
        {
            if (Status != EncounterStatus.InProgress)
                throw new InvalidOperationException("Chỉ có thể hoàn tất lượt khám đang diễn ra (InProgress).");
            if (endAt < CheckInAt)
                throw new ArgumentException("Thời gian kết thúc không thể diễn ra trước thời gian check-in.");

            EndAt = endAt;
            Status = EncounterStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == EncounterStatus.Completed)
                throw new InvalidOperationException("Không thể hủy lượt khám đã hoàn tất.");
            Status = EncounterStatus.Cancelled;
        }
    }
}