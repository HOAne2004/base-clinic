namespace BaseClinic.Domain.Constants
{
    /// <summary>
    /// Khai báo danh sách permission mà một Role hệ thống được cấp mặc định.
    /// Seeder sẽ đọc attribute này để gán permission cho Role.
    /// 
    /// LƯU Ý: Không dùng cho SuperAdmin — SuperAdmin luôn có toàn bộ permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RolePermissionsAttribute : Attribute
    {
        public string[] PermissionCodes { get; }

        public RolePermissionsAttribute(params string[] permissionCodes)
        {
            PermissionCodes = permissionCodes;
        }
    }
}