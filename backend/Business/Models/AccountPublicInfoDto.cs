using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models
{
    public record AccountPublicInfoDto(
        Guid Id,
        string FullName,
        string? Email,
        string PhoneNumber,
        AccountStatus Status,
        bool IsEmailVerified,
        bool IsPhoneNumberVerified,
        List<string> Roles // Chỉ trả về List tên Role để FE điều hướng
    );
}
