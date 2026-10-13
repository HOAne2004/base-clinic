namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// DTO rút gọn cho dropdown/select (chọn bác sĩ khi đặt lịch).
    /// </summary>
    public record DoctorLookupDto(
        Guid Id,
        string FullName,
        string? Avatar,
        string? Title,
        string? Specialization,
        Guid DepartmentId,
        int SortOrder
    );
}