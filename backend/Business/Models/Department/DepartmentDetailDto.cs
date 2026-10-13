namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// DTO chi tiết khoa — dùng cho DANH SÁCH Admin.
    /// Bao gồm số đếm doctors để hiển thị cột "Số bác sĩ".
    /// </summary>
    public record DepartmentDetailDto(
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

        // Stats
        int DoctorCount,
        int ActiveDoctorCount,

        // Audit
        DateTimeOffset CreatedDate,
        DateTimeOffset? UpdatedDate
    );
}