namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// DTO cho DANH SÁCH bác sĩ — dùng cho Admin/Receptionist.
    /// Có thêm DoctorCode, cờ trạng thái, License để quản lý.
    /// Không có audit (xem ở Detail).
    /// </summary>
    public record DoctorAdminListDto(
        Guid Id,
        string DoctorCode,
        string Slug,
        string FullName,
        string? Avatar,
        string? Title,
        string? Specialization,
        decimal? ConsultationFee,
        bool IsActive,
        bool IsAvailableForOnline,
        int SortOrder,

        // Department info
        Guid DepartmentId,
        string? DepartmentName,

        // License status (để admin biết cần gia hạn)
        bool IsLicenseExpiringSoon
    );
}