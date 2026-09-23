using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using System;

namespace BaseClinic.Domain.Entities
{
    public class DoctorSchedule : AggregateRoot
    {
        public Guid DoctorId { get; private set; }
        public DateTime WorkDate { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int SlotDurationMinutes { get; private set; }
        public int MaxPatientPerSlot { get; private set; }
        public int OnlineBookingCapacity { get; private set; }
        public DoctorScheduleStatus Status { get; private set; }

        private DoctorSchedule() { } // Dành cho ORM

        public DoctorSchedule(Guid doctorId, DateTime workDate, DateTime startTime, DateTime endTime,
                              int slotDurationMinutes, int maxPatientPerSlot, int onlineBookingCapacity)
        {
            if (doctorId == Guid.Empty)
                throw new ArgumentException("DoctorId không hợp lệ.", nameof(doctorId));

            DoctorId = doctorId;

            ValidateAndSetScheduleTime(workDate, startTime, endTime, slotDurationMinutes);
            ValidateAndSetCapacity(maxPatientPerSlot, onlineBookingCapacity);

            Status = DoctorScheduleStatus.Available; // Trạng thái mặc định khi tạo mới
        }

        public void UpdateSchedule(DateTime startTime, DateTime endTime, int slotDurationMinutes,
                                   int maxPatientPerSlot, int onlineBookingCapacity)
        {
            // Chỉ cho phép sửa nếu lịch chưa bắt đầu hoặc chưa hoàn thành
            if (Status == DoctorScheduleStatus.Completed || Status == DoctorScheduleStatus.Cancelled)
                throw new InvalidOperationException("Không thể sửa đổi lịch đã kết thúc hoặc đã hủy.");

            ValidateAndSetScheduleTime(WorkDate, startTime, endTime, slotDurationMinutes);
            ValidateAndSetCapacity(maxPatientPerSlot, onlineBookingCapacity);
        }

        private void ValidateAndSetScheduleTime(DateTime workDate, DateTime startTime, DateTime endTime, int slotDurationMinutes)
        {
            if (startTime >= endTime)
                throw new ArgumentException("Thời gian bắt đầu phải diễn ra trước thời gian kết thúc.");

            if (startTime.Date != workDate.Date || endTime.Date != workDate.Date)
                throw new ArgumentException("Thời gian ca khám phải nằm trong cùng ngày làm việc (WorkDate).");

            if (slotDurationMinutes <= 0)
                throw new ArgumentException("Thời lượng mỗi slot khám phải lớn hơn 0 phút.");

            WorkDate = workDate.Date;
            StartTime = startTime;
            EndTime = endTime;
            SlotDurationMinutes = slotDurationMinutes;
        }

        private void ValidateAndSetCapacity(int maxPatient, int onlineCapacity)
        {
            if (maxPatient < 0) throw new ArgumentException("Số bệnh nhân tối đa không được âm.");
            if (onlineCapacity < 0) throw new ArgumentException("Số lượng đặt lịch online không được âm.");
            if (onlineCapacity > maxPatient) throw new ArgumentException("Số lượng đặt online không thể vượt quá sức chứa tối đa của slot.");

            MaxPatientPerSlot = maxPatient;
            OnlineBookingCapacity = onlineCapacity;
        }

        // --- QUẢN LÝ TRẠNG THÁI NGHIỆP VỤ ---
        public void MarkAsFull()
        {
            if (Status != DoctorScheduleStatus.Available)
                throw new InvalidOperationException("Chỉ có thể chuyển sang Full từ trạng thái Available.");
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
                throw new InvalidOperationException("Không thể hoàn thành lịch đã bị hủy.");
            Status = DoctorScheduleStatus.Completed;
        }
    }
}