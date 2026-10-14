namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// DTO cho DANH SÁCH khoa — dùng cho Guest/Patient (landing page).
    /// Ẩn: DepartmentCode, SortOrder, IsActive, audit fields.
    /// </summary>
    public record DepartmentPublicListDto(
        Guid Id,
        string Slug,
        string Name,
        string? ImageUrl,
        string? ShortDescription,
        string? Location,
        string? PhoneNumber,

        bool? IsActive,
        bool OnlineBookingEnabled,

        bool Is24Hours,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,

        int ActiveDoctorCount
    );
}