namespace BaseClinic.Business.Models
{
    public record AccountDto(
    string FullName,
    string PhoneNumber,
    string? Email,
    string Password
);
}
