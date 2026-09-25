namespace BaseClinic.Business.Models
{
    public record DoctorAdminDetailDto(
        Guid Id,
        Guid AccountId,
        string DoctorCode,
        string FullName,
        string PhoneNumber,
        string? Email,
        string DepartmentName,
        string AccountStatus,
        DateTimeOffset? LastLoginAt, // Thêm thông tin bảo mật từ bảng Account
        string? Avatar,
        string? FullAddress,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? LicenseNumber,
        decimal? ConsultationFee,
        string? Description
    );

    public record DoctorListDto(
        Guid Id,
        Guid AccountId,
        string DoctorCode,
        string FullName,
        string PhoneNumber,
        string? Email,
        string DepartmentName,
        string AccountStatus
        );
}