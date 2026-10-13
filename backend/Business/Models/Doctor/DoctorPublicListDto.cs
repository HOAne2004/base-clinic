namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// DTO cho DANH SÁCH bác sĩ công khai (landing page).
    /// Ngắn gọn hơn Profile — chỉ các field để hiển thị card.
    /// </summary>
    public record DoctorPublicListDto(
        Guid Id,
        string Slug,
        string FullName,
        string? Avatar,
        string? Title,
        string? Specialization,
        int? YearsOfExperience,
        decimal? ConsultationFee,
        bool? Gender,

        // Department info
        Guid? DepartmentId,
        string? DepartmentSlug,
        string? DepartmentName
    );
}