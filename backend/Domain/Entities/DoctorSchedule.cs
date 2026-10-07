using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using System;

namespace BaseClinic.Domain.Entities
{
    public class DoctorSchedule : FullAuditedEntity
    {
        private const int MinSlotDuration = 5;
        private const int MaxSlotDuration = 120;
        private const int MaxBookableDaysAhead = 90;

        public Guid DoctorId { get; private set; }
        public DateTime WorkDate { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int SlotDurationMinutes { get; private set; }
        public int MaxPatientPerSlot { get; private set; }
        public int OnlineBookingCapacity { get; private set; }
        public DoctorScheduleStatus Status { get; private set; }

        private DoctorSchedule() { }

        public DoctorSchedule(Guid doctorId, DateTime workDate, DateTime startTime, DateTime endTime,
                              int slotDurationMinutes, int maxPatientPerSlot, int onlineBookingCapacity)
        {
            if (doctorId == Guid.Empty)
                throw new ArgumentException("DoctorId không hợp lệ.");

            DoctorId = doctorId;
            ValidateAndSetScheduleTime(workDate, startTime, endTime, slotDurationMinutes);
            ValidateAndSetCapacity(maxPatientPerSlot, onlineBookingCapacity);

            Status = DoctorScheduleStatus.Available;
        }

        public void UpdateSchedule(DateTime startTime, DateTime endTime,
                                   int slotDurationMinutes, int maxPatientPerSlot, int onlineBookingCapacity)
        {
            if (Status is DoctorScheduleStatus.Completed or DoctorScheduleStatus.Cancelled)
                throw new InvalidOperationException("Không thể sửa lịch đã kết thúc hoặc đã hủy.");

            ValidateAndSetScheduleTime(WorkDate, startTime, endTime, slotDurationMinutes);
            ValidateAndSetCapacity(maxPatientPerSlot, onlineBookingCapacity);
        }

        private void ValidateAndSetScheduleTime(DateTime workDate, DateTime startTime,
                                                DateTime endTime, int slotDurationMinutes)
        {
            var today = DateTime.UtcNow.Date;

            if (workDate.Date < today)
                throw new ArgumentException("Ngày làm việc không thể ở quá khứ.");

            if (workDate.Date > today.AddDays(MaxBookableDaysAhead))
                throw new ArgumentException($"Chỉ có thể tạo lịch trong vòng {MaxBookableDaysAhead} ngày tới.");

            if (startTime >= endTime)
                throw new ArgumentException("Thời gian bắt đầu phải trước thời gian kết thúc.");

            if (startTime.Date != workDate.Date || endTime.Date != workDate.Date)
                throw new ArgumentException("Ca khám phải nằm trong cùng ngày làm việc.");

            if (slotDurationMinutes < MinSlotDuration || slotDurationMinutes > MaxSlotDuration)
                throw new ArgumentException($"Thời lượng slot phải từ {MinSlotDuration} đến {MaxSlotDuration} phút.");

            var totalMinutes = (endTime - startTime).TotalMinutes;
            if (totalMinutes % slotDurationMinutes != 0)
                throw new ArgumentException("Tổng thời gian ca phải chia hết cho thời lượng slot.");

            WorkDate = workDate.Date;
            StartTime = startTime;
            EndTime = endTime;
            SlotDurationMinutes = slotDurationMinutes;
        }

        private void ValidateAndSetCapacity(int maxPatient, int onlineCapacity)
        {
            if (maxPatient < 0) throw new ArgumentException("Số bệnh nhân tối đa không được âm.");
            if (onlineCapacity < 0) throw new ArgumentException("Số lượng đặt online không được âm.");
            if (onlineCapacity > maxPatient)
                throw new ArgumentException("Số lượng đặt online không thể vượt quá sức chứa tối đa.");

            MaxPatientPerSlot = maxPatient;
            OnlineBookingCapacity = onlineCapacity;
        }

        // ==== TRẠNG THÁI ====
        public void MarkAsFull()
        {
            if (Status != DoctorScheduleStatus.Available)
                throw new InvalidOperationException("Chỉ có thể chuyển sang Full từ Available.");
            Status = DoctorScheduleStatus.Full;
        }

        public void CancelSchedule()
        {
            if (Status == DoctorScheduleStatus.Completed)
                throw new InvalidOperationException("Không thể hủy lịch đã hoàn thành.");
            Status = DoctorScheduleStatus.Cancelled;
        }

        public void CompleteSchedule()
        {
            if (Status == DoctorScheduleStatus.Cancelled)
                throw new InvalidOperationException("Không thể hoàn thành lịch đã hủy.");
            Status = DoctorScheduleStatus.Completed;
        }
    }
}