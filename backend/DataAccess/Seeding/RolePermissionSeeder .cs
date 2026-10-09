using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BaseClinic.DataAccess.Seeding
{
    /// <summary>
    /// Gán permission cho Role dựa trên RolePermissionsAttribute.
    /// 
    /// QUY TẮC:
    /// - SuperAdmin: gán TẤT CẢ permissions (không cần attribute).
    /// - Các role khác: đọc attribute [RolePermissions(...)].
    /// 
    /// Seeder SYNC permission của role với attribute:
    /// - Thêm permission có trong attribute nhưng chưa có trong DB.
    /// - Gỡ permission có trong DB nhưng không có trong attribute.
    /// </summary>
    public static class RolePermissionSeeder
    {
        public static async Task SeedAsync(
            ClinicDbContext context,
            CancellationToken cancellationToken = default)
        {
            // 1. Load tất cả role + permission
            var roles = await context.Roles
                .Include(r => r.RolePermissions)
                .ToListAsync(cancellationToken);

            var permissions = await context.Permissions
                .ToDictionaryAsync(p => p.Code, p => p.Id, cancellationToken);

            // 2. Duyệt từng field trong SystemRoles
            var roleFields = typeof(SystemRoles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            foreach (var field in roleFields)
            {
                var roleCode = (string)field.GetRawConstantValue()!;
                var role = roles.FirstOrDefault(r => r.Code == roleCode);

                if (role == null)
                    continue;

                // 3. Xác định permission codes cho role này
                var targetPermissionCodes = GetPermissionCodesForRole(field, roleCode);

                // 4. Chuyển code → Guid
                var targetPermissionIds = targetPermissionCodes
                    .Where(code => permissions.ContainsKey(code))
                    .Select(code => permissions[code])
                    .ToList();

                // 5. Sync — dùng SystemUserId cho GrantedBy
                role.SyncPermissions(targetPermissionIds, SystemConstants.SystemUserId);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Xác định danh sách permission code cho role:
        /// - SuperAdmin: TẤT CẢ permissions trong SystemPermissions.
        /// - Các role khác: đọc từ [RolePermissions] attribute.
        /// </summary>
        private static List<string> GetPermissionCodesForRole(FieldInfo field, string roleCode)
        {
            // SuperAdmin — toàn quyền
            if (roleCode == SystemRoles.SuperAdmin)
            {
                return typeof(SystemPermissions).GetNestedTypes()
                    .SelectMany(t => t.GetFields(
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                        .Select(f => (string)f.GetRawConstantValue()!))
                    .ToList();
            }

            // Các role khác — đọc attribute
            var attr = field.GetCustomAttribute<RolePermissionsAttribute>();
            return attr?.PermissionCodes.ToList() ?? new List<string>();
        }
    }
}