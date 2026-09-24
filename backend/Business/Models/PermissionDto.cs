namespace BaseClinic.Business.Models
{
    // DTO chứa thông tin chi tiết của 1 quyền
    public record PermissionDto
        (Guid Id, string Name, string? Description);
}
