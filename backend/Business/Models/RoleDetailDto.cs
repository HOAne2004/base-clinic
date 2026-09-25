
namespace BaseClinic.Business.Models
{
    public record RoleDetailDto
        (
        Guid Id,
        string Name,
        string? Description,
        bool IsSystemRole
        );
}
