namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// DTO chi tiết khoa cho ADMIN.
    /// Bao gồm danh sách doctors với đầy đủ thông tin quản lý.
    /// </summary>
    public record DepartmentAdminDetailDto(
        Guid Id,
        string Name,
        string DepartmentCode,
        string? ImageUrl,
        string? ShortDescription,
        string? FullDescription,
        string? Location,
        string? PhoneNumber,

        // Operating hours
        bool Is24Hours,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,

        // Sorting + Status
        int SortOrder,
        bool IsActive,

        // Audit
        DateTimeOffset CreatedDate,
        DateTimeOffset? UpdatedDate,

        // Doctors (bao gồm cả inactive để admin quản lý)
        List<DepartmentDoctorSummaryDto> Doctors
    );
}