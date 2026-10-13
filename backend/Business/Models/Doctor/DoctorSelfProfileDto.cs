namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// DTO cho Doctor tự xem profile của mình.
    /// Gần giống Admin Detail nhưng bỏ các field "quản lý" không cần thiết.
    /// </summary>
    public record DoctorSelfProfileDto(
        Guid Id,
        string DoctorCode,
        string Slug,
        Guid AccountId,
        string FullName,
        string? Email,
        string PhoneNumber,

        // Professional
        string? Title,
        string? Specialization,
        int? YearsOfExperience,
        string? Biography,
        string? Languages,

        // Personal
        string? Avatar,
        string? FullAddress,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,

        // License
        string? LicenseNumber,
        DateTime? LicenseIssueDate,
        DateTime? LicenseExpiryDate,
        string? LicenseIssuedBy,
        bool IsLicenseExpiringSoon,

        // Financial & Workload
        decimal? ConsultationFee,
        int? MaxPatientsPerDay,
        DateTime? JoinDate,

        // Description
        string? Description,

        // Status
        bool IsAvailableForOnline,
        bool IsActive,

        // Department
        Guid DepartmentId,
        string? DepartmentName,
        string? DepartmentSlug
    );
}