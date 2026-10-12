namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Records được tạo SAU thời điểm merge → cần admin quyết định thuộc về ai.
    /// </summary>
    public record MergeConflictDto(
        List<ConflictRecordDto> Appointments,
        List<ConflictRecordDto> Encounters,
        List<ConflictRecordDto> Delegations,
        List<ConflictRecordDto> Dependents
    );

    public record ConflictRecordDto(
        Guid Id,
        string RecordType,          // "Appointment", "Encounter", ...
        string DisplayText,         // "LH2026100001 - 15/10/2026"
        Guid CurrentOwnerPatientId, // Survivor hiện tại
        DateTimeOffset CreatedDate
    );

    /// <summary>
    /// Resolution cho 1 conflict record.
    /// </summary>
    public record ConflictResolutionDto(
        string RecordType,
        Guid RecordId,
        ConflictBelongsTo BelongsTo
    );

    public enum ConflictBelongsTo
    {
        Survivor = 1,
        Merged = 2
    }
}