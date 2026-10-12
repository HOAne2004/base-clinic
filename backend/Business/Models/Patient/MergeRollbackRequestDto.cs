using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Thông tin tóm tắt yêu cầu rollback (cho danh sách).
    /// </summary>
    public record MergeRollbackRequestDto(
        Guid Id,
        Guid PatientMergeLogId,
        string RequesterName,
        string Reason,
        MergeRollbackRequestStatus Status,
        DateTimeOffset CreatedDate,
        DateTimeOffset? ReviewedAt,
        string? ReviewNote,
        string SurvivorPatientCode,
        string MergedPatientCode
    );
}