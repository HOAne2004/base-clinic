using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// DTO cho danh sách yêu cầu hủy ca.
    /// </summary>
    public record ScheduleCancellationRequestDto(
        Guid Id,
        Guid DoctorScheduleId,
        DateOnly WorkDate,
        TimeOnly StartTime,
        TimeOnly EndTime,

        // Doctor + Department info
        Guid DoctorId,
        string DoctorName,
        Guid DepartmentId,
        string? DepartmentName,

        // Requester
        Guid RequestedByAccountId,
        string RequesterName,
        string Reason,

        // Status
        ScheduleCancellationRequestStatus Status,
        DateTimeOffset CreatedDate,

        // Review
        Guid? ReviewedByAccountId,
        string? ReviewerName,
        DateTimeOffset? ReviewedAt,
        string? ReviewNote
    );
}