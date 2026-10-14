namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// DTO cho slot còn chỗ — dùng cho đặt lịch.
    /// Bao gồm cả doctor info để FE hiển thị khi user muốn chọn bác sĩ.
    /// </summary>
    public record AvailableSlotDto(
        Guid SlotId,
        Guid DoctorScheduleId,
        DateOnly WorkDate,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int AvailableCount,

        // Doctor info
        Guid DoctorId,
        string DoctorName,
        string? DoctorAvatar,
        string? DoctorTitle,
        string? Specialization,

        // Department info
        Guid DepartmentId,
        string? DepartmentName
    );
}