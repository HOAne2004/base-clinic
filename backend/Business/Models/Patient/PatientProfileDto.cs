namespace BaseClinic.Business.Models.Patient
{
    public record PatientProfileDto(
        Guid Id,
        string PatientCode,
        string? FullName,
        string? Avatar,
        string? FullAddress,
        string? IdentityNumber,
        bool? Gender,
        DateTime? DateOfBirth
    );
}
