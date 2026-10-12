namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Thông tin tối giản cho danh sách bệnh nhân.
    /// Chỉ chứa thông tin user cần thấy để tra cứu / nhận diện.
    /// </summary>
    public record PatientListItemDto(
        Guid Id,
        string PatientCode,
        string FullName,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? HealthInsuranceNumber,
        string? PhoneNumber,        // Từ Account (nếu có)
        string? AddressFullText     // Chuỗi address đã ghép, không cần tách
    );
}