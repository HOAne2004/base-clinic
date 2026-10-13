namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// DTO chi tiết khoa cho ADMIN.
    /// Bao gồm đầy đủ thông tin + audit + danh sách doctors.
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

        bool Is24Hours,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,

        int SortOrder,
        bool IsActive,

        // Audit — Admin cần để truy vết
        DateTimeOffset CreatedDate,
        DateTimeOffset? UpdatedDate,

        // Danh sách doctors
        List<DepartmentDoctorSummaryDto> Doctors
    );
}