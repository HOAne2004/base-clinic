using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Chi tiết 1 yêu cầu hủy — bao gồm schedule detail.
    /// </summary>
    public record ScheduleCancellationRequestDetailDto(
        Guid Id,
        ScheduleCancellationRequestStatus Status,
        string Reason,
        DateTimeOffset CreatedDate,

        Guid RequestedByAccountId,
        string RequesterName,

        Guid? ReviewedByAccountId,
        string? ReviewerName,
        DateTimeOffset? ReviewedAt,
        string? ReviewNote,

        DoctorScheduleListDto Schedule
    );
}