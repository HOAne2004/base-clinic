namespace BaseClinic.Business.Models
{
    public record DoctorDto(
    Guid DepartmentId,
    string DoctorCode,
    string? Avatar,
    string? FullAddress,
    bool? Gender,
    DateTime? DateOfBirth,
    string? IdentityNumber,
    string? LicenseNumber,
    decimal? ConsultationFee,
    string? Description
);
}
