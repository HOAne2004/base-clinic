using BaseClinic.Business.Models.Common;

namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Thông tin chi tiết bệnh nhân.
    /// </summary>
    public record PatientDto(
        Guid Id,
        string PatientCode,
        string FullName,
        string? AvatarUrl,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? HealthInsuranceNumber,

        // Address — 2 dạng
        string? AddressFullText,      // Hiển thị
        AddressDto? Address,          // Form edit

        // Quan hệ
        Guid? AccountId,
        Guid? PrimaryPatientId,
        string? PhoneNumber,          // Từ Account
        string? Email,                // Từ Account

        // Audit
        DateTimeOffset CreatedDate,
        DateTimeOffset? UpdatedDate
    );
}