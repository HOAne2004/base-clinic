using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BaseClinic.DataAccess.Seeding
{
    /// <summary>
    /// Seed Permission từ SystemPermissions constants.
    /// Idempotent: chạy nhiều lần không tạo trùng.
    /// </summary>
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(
            ClinicDbContext context,
            CancellationToken cancellationToken = default)
        {
            // 1. Đọc tất cả nested class trong SystemPermissions
            var nestedTypes = typeof(SystemPermissions).GetNestedTypes();

            // 2. Gom toàn bộ permission code + metadata
            var permissionMetadatas = new List<(string Code, string Name, string Resource, string Action, string Description, int DisplayOrder)>();

            foreach (var type in nestedTypes)
            {
                var fields = type.GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

                foreach (var field in fields)
                {
                    var code = (string)field.GetRawConstantValue()!;
                    var attr = field.GetCustomAttribute<PermissionInfoAttribute>();

                    if (attr == null)
                        continue;

                    // Name lấy từ Description (hoặc có thể tách riêng nếu muốn)
                    // Format: "Tạo lịch hẹn khám bệnh mới."
                    permissionMetadatas.Add((
                        Code: code,
                        Name: attr.Description,
                        Resource: attr.Resource,
                        Action: attr.Action,
                        Description: attr.Description,
                        DisplayOrder: attr.DisplayOrder
                    ));
                }
            }

            // 3. Lấy code đã có trong DB
            var existingCodes = await context.Permissions
                .Select(p => p.Code)
                .ToListAsync(cancellationToken);
            var existingSet = existingCodes.ToHashSet();

            // 4. Tạo mới những permission chưa có
            var toAdd = new List<Permission>();

            foreach (var meta in permissionMetadatas)
            {
                if (existingSet.Contains(meta.Code))
                    continue;

                var permission = new Permission(
                    code: meta.Code,
                    name: meta.Name,
                    resource: meta.Resource,
                    action: meta.Action,
                    description: meta.Description,
                    displayOrder: meta.DisplayOrder);

                toAdd.Add(permission);
            }

            if (toAdd.Any())
            {
                context.Permissions.AddRange(toAdd);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}