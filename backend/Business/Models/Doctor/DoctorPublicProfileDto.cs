namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// DTO chi tiết bác sĩ cho GUEST/PATIENT.
    /// URL: GET /api/doctors/{slug}
    /// Ẩn: License, CCCD, Address, SortOrder, audit, các cờ nội bộ.
    /// </summary>
    public record DoctorPublicProfileDto(
        Guid Id,
        string Slug,
        string FullName,              // Từ Account.FullName
        string? Avatar,
        string? Title,                // BS, ThS, TS, PGS, GS
        string? Specialization,       // Chuyên khoa sâu
        string? Biography,
        int? YearsOfExperience,
        string? Languages,            // "vi,en"
        decimal? ConsultationFee,
        bool? Gender,
        int? Age,                     // Tính từ DOB, không expose DOB
        string? Description,

        // Department info
        Guid? DepartmentId,
        string? DepartmentSlug,
        string? DepartmentName
    );
}