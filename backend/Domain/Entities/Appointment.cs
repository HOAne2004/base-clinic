using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Domain.Entities
{
    public class Appointment : AggregateRoot
    {
        public Guid PatientId { get; private set; }
        public Guid? DepartmentId { get; private set; }
        public DateTime AppointmentDate { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public string Reason { get; private set; } = string.Empty;
        public Guid? RequestedDoctorId { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public string? CancelReason { get; private set; }

        private Appointment() { } // Dành cho EF Core

        public Appointment(Guid patientId, Guid? departmentId, DateTime appointmentDate, TimeOnly startTime, TimeOnly endTime, string reason, Guid? requestedDoctorId)
        {
            if (appointmentDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Ngày hẹn không được ở trong quá khứ.");
            if (startTime >= endTime)
                throw new ArgumentException("Thời gian bắt đầu phải trước thời gian kết thúc.");

            PatientId = patientId;
            DepartmentId = departmentId;
            AppointmentDate = appointmentDate.Date;
            StartTime = startTime;
            EndTime = endTime;
            Reason = reason ?? string.Empty;
            RequestedDoctorId = requestedDoctorId;
            Status = AppointmentStatus.Confirmed; // Trạng thái mặc định khi tạo mới
        }

        public void Reschedule(DateTime newDate, TimeOnly newStartTime, TimeOnly newEndTime)
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Không thể dời lịch trình cho lịch hẹn đã hoàn thành hoặc đã hủy.");
            if (newDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Ngày hẹn không được ở trong quá khứ.");
            if (newStartTime >= newEndTime)
                throw new ArgumentException("Thời gian bắt đầu phải trước thời gian kết thúc.");

            AppointmentDate = newDate.Date;
            StartTime = newStartTime;
            EndTime = newEndTime;
        }

        // --- VÒNG ĐỜI TRẠNG THÁI (STATE MACHINE) ---
        public void CheckIn()
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Chỉ có thể check-in lịch hẹn đã được xác nhận (Confirmed).");
            Status = AppointmentStatus.CheckedIn;
        }

        public void Complete()
        {
            if (Status != AppointmentStatus.CheckedIn)
                throw new InvalidOperationException("Chỉ có thể hoàn thành lịch hẹn đã check-in.");
            Status = AppointmentStatus.Completed;
        }

        public void MarkAsNoShow()
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Chỉ có thể đánh dấu No-Show cho lịch hẹn chưa đến (Confirmed).");
            Status = AppointmentStatus.NoShow;
        }

        public void Cancel(string cancelReason)
        {
            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Không thể hủy lịch hẹn đã hoàn thành.");
            if (string.IsNullOrWhiteSpace(cancelReason))
                throw new ArgumentException("Bắt buộc phải cung cấp lý do hủy lịch.");

            Status = AppointmentStatus.Cancelled;
            CancelReason = cancelReason.Trim();
        }
    }
}