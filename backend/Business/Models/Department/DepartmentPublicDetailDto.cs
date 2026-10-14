namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// DTO chi tiết khoa cho GUEST/PATIENT.
    /// Ẩn thông tin nội bộ: SortOrder, IsActive, audit, doctors inactive.
    /// </summary>
    public record DepartmentPublicDetailDto(
        Guid Id,
        string Slug,
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

        bool OnlineBookingEnabled,
        // Chỉ doctors đang active
        List<DepartmentDoctorPublicDto> Doctors
    );
}