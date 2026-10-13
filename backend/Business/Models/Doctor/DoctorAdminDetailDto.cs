namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// DTO chi tiết bác sĩ cho ADMIN.
    /// Bao gồm đầy đủ thông tin + audit + license.
    /// </summary>
    public record DoctorAdminDetailDto(
        Guid Id,
        string DoctorCode,
        string Slug,
        Guid AccountId,
        string FullName,               // Từ Account
        string? Email,                 // Từ Account
        string PhoneNumber,            // Từ Account

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

        // Financial & Workload
        decimal? ConsultationFee,
        int? MaxPatientsPerDay,
        DateTime? JoinDate,

        // Description
        string? Description,

        // Status
        bool IsAvailableForOnline,
        bool IsActive,
        int SortOrder,

        // Department
        Guid DepartmentId,
        string? DepartmentName,
        string? DepartmentSlug,

        // Audit
        DateTimeOffset CreatedDate,
        DateTimeOffset? UpdatedDate
    );
}