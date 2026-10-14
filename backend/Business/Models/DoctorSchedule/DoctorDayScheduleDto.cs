using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Lịch làm việc của TẤT CẢ bác sĩ trong 1 khoa vào 1 ngày.
    /// Dùng cho Lễ tân xem tổng quan.
    /// </summary>
    public record DoctorDayScheduleDto(
        DateOnly WorkDate,
        Guid DepartmentId,
        string? DepartmentName,
        List<DoctorDayItemDto> Doctors
    );

    /// <summary>
    /// Lịch của 1 bác sĩ trong ngày — có thể có nhiều ca.
    /// </summary>
    public record DoctorDayItemDto(
        Guid DoctorId,
        string DoctorName,
        string DoctorCode,
        string? Avatar,
        string? Title,
        string? Specialization,
        List<DoctorScheduleSummaryItemDto> Schedules
    );

    /// <summary>
    /// Tóm tắt 1 ca làm việc trong ngày.
    /// </summary>
    public record DoctorScheduleSummaryItemDto(
        Guid ScheduleId,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int TotalSlots,
        int BookedSlots,
        int AvailableSlots,
        DoctorScheduleStatus Status
    );
}