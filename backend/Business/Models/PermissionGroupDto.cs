namespace BaseClinic.Business.Models
{
    // DTO gom nhóm các quyền theo Resource (VD: Nhóm "Appointment" có quyền Book, Cancel...)
    public class PermissionGroupDto
    {
        public string Resource { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}
