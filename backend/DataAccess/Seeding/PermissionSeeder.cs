using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BaseClinic.DataAccess.Seeding
{
    public static class PermissionSeeder
    {
        public static async Task SeedPermissionsAsync(ClinicDbContext context)
        {
            var permissionsToAdd = new List<Permission>();

            // 1. Quét các class con (Appointment, Encounter...)
            var nestedTypes = typeof(SystemPermissions).GetNestedTypes();

            foreach (var type in nestedTypes)
            {
                // Lấy tên class làm Resource (VD: "Appointment")
                string resourceName = type.Name;

                // Lấy các biến const bên trong class
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                 .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

                foreach (var field in fields)
                {
                    // Lấy mã quyền (VD: "Appointment.Book")
                    string permissionName = (string)field.GetRawConstantValue()!;

                    // Lấy mô tả từ Attribute
                    var attribute = field.GetCustomAttribute<PermissionInfoAttribute>();
                    string description = attribute?.Description ?? string.Empty;

                    // Kiểm tra xem Database đã có quyền này chưa
                    bool exists = await context.Permissions.AnyAsync(p => p.Name == permissionName);

                    if (!exists)
                    {
                        permissionsToAdd.Add(new Permission(permissionName, resourceName, description));
                    }
                }
            }

            // 2. Lưu vào CSDL
            if (permissionsToAdd.Any())
            {
                context.Permissions.AddRange(permissionsToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}