namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Thông tin tóm tắt của dependent.
    /// </summary>
    public record DependentListItemDto(
        Guid Id,
        string PatientCode,
        string FullName,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? HealthInsuranceNumber,
        string? Relationship,
        DateTimeOffset CreatedDate
    );
}