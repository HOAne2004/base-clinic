using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// DTO chi tiết ca làm việc — có đầy đủ slots.
    /// </summary>
    public record DoctorScheduleDetailDto(
        Guid Id,
        DateOnly WorkDate,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int SlotDurationMinutes,
        int MaxPatientPerSlot,
        bool OnlineBookingEnabled,
        DoctorScheduleStatus Status,
        string? Note,

        // Cancellation info
        DateTimeOffset? CancelledAt,
        Guid? CancelledBy,
        string? CancelReason,

        // Doctor info
        Guid DoctorId,
        string DoctorName,
        string DoctorCode,

        // Department info
        Guid DepartmentId,
        string? DepartmentName,

        // Audit
        DateTimeOffset CreatedDate,
        DateTimeOffset? UpdatedDate,

        // Slots
        List<DoctorScheduleItemDto> Items
    );
}