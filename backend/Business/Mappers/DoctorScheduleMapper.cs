using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Mappers
{
    /// <summary>
    /// Helper chuyển DoctorSchedule entity sang các DTO.
    /// 
    /// QUY ƯỚC:
    /// - Doctor không có FullName → phải truyền doctorFullName riêng.
    /// - Department có thể null (nếu chỉ cần hiển thị time).
    /// - Các DTO list/detail cần tổng hợp TotalSlots/BookedSlots/AvailableSlots.
    /// </summary>
    public static class DoctorScheduleMapper
    {
        // ==================== LIST ====================

        /// <summary>
        /// Map Schedule → ListDto (cho danh sách ca).
        /// </summary>
        public static DoctorScheduleListDto ToListItemDto(
            DoctorSchedule schedule,
            Doctor doctor,
            string doctorFullName,
            string doctorCode,
            Department? department)
        {
            var items = schedule.Items.ToList();

            var totalSlots = items.Count;
            var bookedSlots = items.Sum(i => i.BookedCount);
            var availableSlots = items.Sum(i => i.AvailableSlots);

            return new DoctorScheduleListDto(
                Id: schedule.Id,
                WorkDate: schedule.WorkDate,
                StartTime: schedule.StartTime,
                EndTime: schedule.EndTime,
                SlotDurationMinutes: schedule.SlotDurationMinutes,
                MaxPatientPerSlot: schedule.MaxPatientPerSlot,

                TotalSlots: totalSlots,
                BookedSlots: bookedSlots,
                AvailableSlots: availableSlots,

                OnlineBookingEnabled: schedule.OnlineBookingEnabled,
                Status: schedule.Status,
                Note: schedule.Note,

                DoctorId: schedule.DoctorId,
                DoctorName: doctorFullName,
                DoctorCode: doctorCode,

                DepartmentId: doctor.DepartmentId,
                DepartmentName: department?.Name);
        }

        // ==================== DETAIL ====================

        /// <summary>
        /// Map Schedule → DetailDto (có items).
        /// </summary>
        public static DoctorScheduleDetailDto ToDetailDto(
            DoctorSchedule schedule,
            Doctor doctor,
            string doctorFullName,
            string doctorCode,
            Department? department)
        {
            var itemDtos = schedule.Items
                .OrderBy(i => i.StartTime)
                .Select(ToItemDto)
                .ToList();

            return new DoctorScheduleDetailDto(
                Id: schedule.Id,
                WorkDate: schedule.WorkDate,
                StartTime: schedule.StartTime,
                EndTime: schedule.EndTime,
                SlotDurationMinutes: schedule.SlotDurationMinutes,
                MaxPatientPerSlot: schedule.MaxPatientPerSlot,
                OnlineBookingEnabled: schedule.OnlineBookingEnabled,
                Status: schedule.Status,
                Note: schedule.Note,

                CancelledAt: schedule.CancelledAt,
                CancelledBy: schedule.CancelledBy,
                CancelReason: schedule.CancelReason,

                DoctorId: schedule.DoctorId,
                DoctorName: doctorFullName,
                DoctorCode: doctorCode,

                DepartmentId: doctor.DepartmentId,
                DepartmentName: department?.Name,

                CreatedDate: schedule.CreatedDate,
                UpdatedDate: schedule.UpdatedDate,

                Items: itemDtos);
        }

        // ==================== ITEM ====================

        /// <summary>
        /// Map DoctorScheduleItem → ItemDto.
        /// </summary>
        public static DoctorScheduleItemDto ToItemDto(DoctorScheduleItem item)
        {
            return new DoctorScheduleItemDto(
                Id: item.Id,
                StartTime: item.StartTime,
                EndTime: item.EndTime,
                Capacity: item.Capacity,
                BookedCount: item.BookedCount,
                AvailableCount: item.AvailableSlots,
                Status: item.Status);
        }

        // ==================== AVAILABLE SLOT ====================

        /// <summary>
        /// Map Item (slot) → AvailableSlotDto.
        /// Chỉ gọi khi item còn chỗ.
        /// </summary>
        public static AvailableSlotDto ToAvailableSlotDto(
            DoctorScheduleItem item,
            DoctorSchedule schedule,
            Doctor doctor,
            string doctorFullName,
            Department department)
        {

            return new AvailableSlotDto(
                SlotId: item.Id,
                DoctorScheduleId: schedule.Id,
                WorkDate: schedule.WorkDate,
                StartTime: item.StartTime,
                EndTime: item.EndTime,
                AvailableCount: item.AvailableSlots,

                DoctorId: doctor.Id,
                DoctorName: doctorFullName,
                DoctorAvatar: doctor.Avatar,
                DoctorTitle: doctor.Title,
                Specialization: doctor.Specialization,

                DepartmentId: doctor.DepartmentId,
                DepartmentName: department?.Name);
        }

        // ==================== SUMMARY ====================

        /// <summary>
        /// Map Schedule → SummaryDto (cho DoctorDayScheduleDto).
        /// </summary>
        public static DoctorScheduleSummaryItemDto ToSummaryItemDto(
            DoctorSchedule schedule)
        {
            var items = schedule.Items.ToList();

            return new DoctorScheduleSummaryItemDto(
                ScheduleId: schedule.Id,
                StartTime: schedule.StartTime,
                EndTime: schedule.EndTime,
                TotalSlots: items.Count,
                BookedSlots: items.Sum(i => i.BookedCount),
                AvailableSlots: items.Sum(i => i.AvailableSlots),
                Status: schedule.Status);
        }
    }
}