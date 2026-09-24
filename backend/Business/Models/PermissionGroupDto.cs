namespace BaseClinic.Business.Models
{
    // DTO gom nhóm các quyền theo Resource (VD: Nhóm "Appointment" có quyền Book, Cancel...)
    public record PermissionGroupDto (string Resource, List<PermissionDto> Permissions);

}
