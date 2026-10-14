using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Lịch hẹn khám bệnh.
    /// 
    /// SNAPSHOT: Lưu ContactPhone/ContactName/ContactRelationship tại thời điểm đặt.
    /// Không sync với Patient hay Account — vì mỗi lần đặt có thể dùng số khác nhau.
    /// </summary>
    public class Appointment : FullAuditedEntity
    {
        // ==================== ĐỊNH DANH ====================
        /// <summary>Mã lịch hẹn — unique. VD: "LH2026100001".</summary>
        public string AppointmentCode { get; private set; } = string.Empty;

        // ==================== LIÊN KẾT ====================
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = null!;

        /// <summary>Bác sĩ yêu cầu (user chọn khi đặt). Null nếu không yêu cầu cụ thể.</summary>
        public Guid? RequestedDoctorId { get; private set; }
        public Doctor? RequestedDoctor { get; private set; }

        /// <summary>Bác sĩ thực sự khám (được phân công sau). Null nếu chưa phân công.</summary>
        public Guid? DoctorId { get; private set; }
        public Doctor? Doctor { get; private set; }

        /// <summary>Khoa khám.</summary>
        public Guid? DepartmentId { get; private set; }
        public Department? Department { get; private set; }

        // ==================== THỜI GIAN ====================
        public DateTime AppointmentDate { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }

        // ==================== THÔNG TIN LIÊN HỆ (SNAPSHOT) ====================
        /// <summary>SĐT liên hệ cho lịch hẹn này. Bắt buộc.</summary>
        public string ContactPhone { get; private set; } = string.Empty;

        /// <summary>Tên người liên hệ. Null → fallback về Patient.FullName khi hiển thị.</summary>
        public string? ContactName { get; private set; }

        /// <summary>Quan hệ với bệnh nhân. Null → chưa xác định.</summary>
        public PatientRelationshipType? ContactRelationship { get; private set; }

        // ==================== NGHIỆP VỤ ====================
        /// <summary>Lý do khám.</summary>
        public string? Reason { get; private set; }

        public AppointmentStatus Status { get; private set; }

        /// <summary>Lý do hủy. Chỉ có giá trị khi Status = Cancelled.</summary>
        public string? CancelReason { get; private set; }

        /// <summary>
        /// Slot đã giữ chỗ cho appointment này.
        /// - Null: walk-in không qua slot (đến trực tiếp).
        /// - Có: appointment đặt qua hệ thống → giữ chỗ slot của BS.
        /// </summary>
        public Guid? DoctorScheduleItemId { get; private set; }
        public DoctorScheduleItem? DoctorScheduleItem { get; private set; }

        // ==================== CONSTRUCTOR ====================
        private Appointment() { } // EF Core

        /// <summary>
        /// Tạo lịch hẹn mới.
        /// </summary>
        /// <param name="appointmentCode">Mã lịch hẹn — sinh bởi IAppointmentCodeGenerator.</param>
        /// <param name="patientId">Bệnh nhân.</param>
        /// <param name="contactPhone">SĐT liên hệ (bắt buộc).</param>
        /// <param name="appointmentDate">Ngày hẹn.</param>
        /// <param name="startTime">Giờ bắt đầu.</param>
        /// <param name="endTime">Giờ kết thúc.</param>
        /// <param name="departmentId">Khoa (optional).</param>
        /// <param name="requestedDoctorId">Bác sĩ yêu cầu (optional).</param>
        /// <param name="contactName">Tên người liên hệ (optional).</param>
        /// <param name="contactRelationship">Quan hệ (optional).</param>
        /// <param name="reason">Lý do khám (optional).</param>
        public Appointment(
            string appointmentCode,
            Guid patientId,
            string contactPhone,
            DateTime appointmentDate,
            TimeOnly startTime,
            TimeOnly endTime,
            Guid? departmentId = null,
            Guid? requestedDoctorId = null,
            string? contactName = null,
            PatientRelationshipType? contactRelationship = null,
            string? reason = null,
            bool isFromReceptionist = false,
            Guid? doctorScheduleItemId = null)
        {
            SetAppointmentCode(appointmentCode);

            if (patientId == Guid.Empty)
                throw new DomainException("PatientId không hợp lệ.");

            SetContactPhone(contactPhone);
            SetSchedule(appointmentDate, startTime, endTime);

            PatientId = patientId;
            DepartmentId = departmentId;
            RequestedDoctorId = requestedDoctorId;
            ContactName = string.IsNullOrWhiteSpace(contactName) ? null : contactName.Trim();
            ContactRelationship = contactRelationship;
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();

            Status = isFromReceptionist
                ? AppointmentStatus.Confirmed
                : AppointmentStatus.Pending;
            DoctorScheduleItemId = doctorScheduleItemId;
        }

        // ==================== PRIVATE SETTERS ====================
        private void SetAppointmentCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Mã lịch hẹn không được để trống.");
            if (code.Length > 50)
                throw new DomainException("Mã lịch hẹn không vượt quá 50 ký tự.");

            AppointmentCode = code.Trim();
        }

        private void SetContactPhone(string contactPhone)
        {
            if (string.IsNullOrWhiteSpace(contactPhone))
                throw new DomainException("SĐT liên hệ không được để trống.");
            if (contactPhone.Length > 20)
                throw new DomainException("SĐT liên hệ không vượt quá 20 ký tự.");

            ContactPhone = contactPhone.Trim();
        }

        private void SetSchedule(DateTime date, TimeOnly start, TimeOnly end)
        {
            if (date.Date < DateTime.UtcNow.Date)
                throw new DomainException("Ngày hẹn không được ở trong quá khứ.");
            if (start >= end)
                throw new DomainException("Thời gian bắt đầu phải trước thời gian kết thúc.");

            AppointmentDate = date.Date;
            StartTime = start;
            EndTime = end;
        }
        internal void SetPatientIdInternal(Guid patientId)
        {
            PatientId = patientId;
        }

        // ==================== UPDATE METHODS ====================
        /// <summary>Đổi thông tin liên hệ.</summary>
        public void UpdateContact(
            string contactPhone,
            string? contactName,
            PatientRelationshipType? contactRelationship)
        {
            if (Status is AppointmentStatus.Completed
                     or AppointmentStatus.Cancelled
                     or AppointmentStatus.NoShow)
                throw new DomainException("Không thể đổi thông tin liên hệ của lịch hẹn đã kết thúc.");

            SetContactPhone(contactPhone);
            ContactName = string.IsNullOrWhiteSpace(contactName) ? null : contactName.Trim();
            ContactRelationship = contactRelationship;
        }

        /// <summary>Đổi lý do khám.</summary>
        public void UpdateReason(string? reason)
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
                throw new DomainException("Không thể đổi lý do khám của lịch hẹn đã kết thúc.");

            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        }

        /// <summary>Dời lịch hẹn. Chỉ cho phép khi Status = Confirmed.</summary>
        public void Reschedule(DateTime newDate, TimeOnly newStartTime, TimeOnly newEndTime)
        {
            if (Status is not (AppointmentStatus.Pending or AppointmentStatus.Confirmed))
                throw new DomainException(
                    $"Chỉ có thể dời lịch hẹn đang Pending hoặc Confirmed. " +
                    $"Trạng thái hiện tại: {Status}.");

            SetSchedule(newDate, newStartTime, newEndTime);
            Status = AppointmentStatus.Confirmed;
        }

        /// <summary>Phân công bác sĩ thực sự khám (bởi lễ tân/admin).</summary>
        public void AssignDoctor(Guid doctorId)
        {
            if (doctorId == Guid.Empty)
                throw new DomainException("DoctorId không hợp lệ.");

            if (Status is AppointmentStatus.Completed
                     or AppointmentStatus.Cancelled
                     or AppointmentStatus.NoShow)
                throw new DomainException("Không thể phân công bác sĩ cho lịch hẹn đã kết thúc.");

            DoctorId = doctorId;
        }

        // ==================== STATE MACHINE ====================
        /// <summary>
        /// Xác nhận lịch hẹn — chuyển từ Pending sang Confirmed.
        /// Dùng khi lễ tân/bác sĩ xác nhận lịch đặt online.
        /// </summary>
        public void Confirm()
        {
            if (Status != AppointmentStatus.Pending)
                throw new DomainException(
                    $"Chỉ có thể xác nhận lịch hẹn đang Pending. " +
                    $"Trạng thái hiện tại: {Status}.");

            Status = AppointmentStatus.Confirmed;
        }

        /// <summary>Check-in bệnh nhân khi đến.</summary>
        public void CheckIn()
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new DomainException(
                    $"Chỉ có thể check-in lịch hẹn đang Confirmed. " +
                    $"Trạng thái hiện tại: {Status}.");

            Status = AppointmentStatus.CheckedIn;
        }

        /// <summary>Hoàn thành lịch hẹn (sau khi khám xong).</summary>
        public void Complete()
        {
            if (Status != AppointmentStatus.CheckedIn)
                throw new DomainException(
                    $"Chỉ có thể hoàn thành lịch hẹn đã check-in. " +
                    $"Trạng thái hiện tại: {Status}.");

            Status = AppointmentStatus.Completed;
        }

        /// <summary>Đánh dấu bệnh nhân không đến (no-show).</summary>
        public void MarkAsNoShow()
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new DomainException(
                    $"Chỉ có thể đánh dấu No-Show cho lịch hẹn đang Confirmed. " +
                    $"Trạng thái hiện tại: {Status}.");

            Status = AppointmentStatus.NoShow;
        }

        /// <summary>Hủy lịch hẹn.</summary>
        public void Cancel(string cancelReason)
        {
            // Cho phép hủy từ Pending HOẶC Confirmed
            if (Status is not (AppointmentStatus.Pending or AppointmentStatus.Confirmed))
                throw new DomainException(
                    $"Chỉ có thể hủy lịch hẹn đang Pending hoặc Confirmed. " +
                    $"Trạng thái hiện tại: {Status}.");

            if (string.IsNullOrWhiteSpace(cancelReason))
                throw new DomainException("Bắt buộc phải cung cấp lý do hủy lịch.");

            Status = AppointmentStatus.Cancelled;
            CancelReason = cancelReason.Trim();
        }
    }
}