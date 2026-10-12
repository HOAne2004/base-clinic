namespace BaseClinic.Business.Models.Patient
{
    public record MergeLogDetailDto(
        Guid Id,
        Guid SurvivorPatientId,
        string SurvivorPatientCode,
        string SurvivorFullName,
        Guid MergedPatientId,
        string MergedPatientCode,
        string MergedFullName,
        string Reason,
        Guid MergedByAccountId,
        DateTimeOffset MergedAt,
        bool IsRolledBack,
        DateTimeOffset? RolledBackAt,
        Guid? RolledBackByAccountId,
        string? RollbackReason
    );
}