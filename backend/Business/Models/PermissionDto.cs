namespace BaseClinic.Business.Models
{
    // DTO chứa thông tin chi tiết của 1 quyền
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
