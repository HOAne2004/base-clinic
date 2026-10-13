namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// Thông tin tóm tắt bác sĩ trong khoa — dùng cho Admin.
    /// Bao gồm cả trạng thái IsActive, SortOrder để admin quản lý.
    /// </summary>
    public record DepartmentDoctorSummaryDto(
        Guid Id,
        string DoctorCode,
        string FullName,            // Từ Account.FullName
        string? Avatar,
        string? Title,              // BS, ThS, TS, PGS, GS
        string? Specialization,     // Chuyên khoa sâu
        bool IsActive,
        bool IsAvailableForOnline,
        decimal? ConsultationFee,
        int SortOrder
    );
}