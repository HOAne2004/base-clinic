namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// DTO chi tiết khoa — dùng cho DANH SÁCH Admin.
    /// Bao gồm số đếm doctors để hiển thị cột "Số bác sĩ".
    /// </summary>
    public record DepartmentAdminListDto(
        Guid Id,
        string Name,
        string DepartmentCode,
        string? PhoneNumber,

        bool Is24Hours,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,

        bool IsActive,
        bool OnlineBookingEnabled,
        int SortOrder,

        int ActiveDoctorCount
    );
}