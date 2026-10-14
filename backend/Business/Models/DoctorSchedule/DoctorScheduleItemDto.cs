using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// DTO cho 1 slot cụ thể trong ca.
    /// </summary>
    public record DoctorScheduleItemDto(
        Guid Id,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int Capacity,
        int BookedCount,
        int AvailableCount,
        DoctorScheduleItemStatus Status
    );
}