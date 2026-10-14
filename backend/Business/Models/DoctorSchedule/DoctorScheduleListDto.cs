using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// DTO cho DANH SÁCH ca làm việc — dùng cho Admin/Doctor.
    /// Có tổng hợp số slot để hiển thị nhanh.
    /// </summary>
    public record DoctorScheduleListDto(
        Guid Id,
        DateOnly WorkDate,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int SlotDurationMinutes,
        int MaxPatientPerSlot,

        // Tổng hợp slots
        int TotalSlots,
        int BookedSlots,
        int AvailableSlots,

        bool OnlineBookingEnabled,
        DoctorScheduleStatus Status,
        string? Note,

        // Doctor info
        Guid DoctorId,
        string DoctorName,
        string DoctorCode,

        // Department info
        Guid DepartmentId,
        string? DepartmentName
    );
}