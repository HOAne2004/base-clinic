namespace BaseClinic.Business.Models
{
    public class TokenUser
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid SecurityStamp { get; set; }
        public Guid SessionId { get; set; }

        // Giữ lại Roles nếu Frontend cần để hiển thị menu
        public List<string> Roles { get; set; } = new();

        // Danh sách mã quyền để phục vụ RBAC động
        public List<string> Permissions { get; set; } = new();
    }
}
