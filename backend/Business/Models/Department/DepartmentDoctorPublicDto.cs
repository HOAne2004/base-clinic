namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// Thông tin bác sĩ CÔNG KHAI — dùng cho Guest/Patient.
    /// Ẩn: DoctorCode, IsActive, SortOrder, License, AccountId...
    /// </summary>
    public record DepartmentDoctorPublicDto(
        Guid Id,
        string FullName,
        string? Avatar,
        string? Title,
        string? Specialization,
        string? Biography,
        int? YearsOfExperience,
        decimal? ConsultationFee
    );
}