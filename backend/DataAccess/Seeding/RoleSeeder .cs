using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BaseClinic.DataAccess.Seeding
{
    /// <summary>
    /// Seed các Role hệ thống từ SystemRoles constants.
    /// Idempotent: chạy nhiều lần không tạo trùng.
    /// </summary>
    public static class RoleSeeder
    {
        /// <summary>
        /// Metadata cho từng role.
        /// 
        /// Priority    — dùng cho logic (số lớn = quyền cao hơn khi conflict).
        /// DisplayOrder — dùng cho UI (số nhỏ = hiển thị trước).
        /// 
        /// DisplayOrder khác Priority vì:
        /// - Patient phổ biến nhất → hiển thị đầu (DisplayOrder = 1).
        /// - SuperAdmin hiếm khi chọn → hiển thị cuối (DisplayOrder = 5).
        /// </summary>
        private static readonly Dictionary<string, (string Name, string Description, int Priority, int DisplayOrder)> RoleMetadata = new()
        {
            [SystemRoles.SuperAdmin] = ("Siêu quản trị", "Toàn quyền hệ thống.", 100, 5),
            [SystemRoles.Admin] = ("Quản trị viên", "Quản lý user, role, department, doctor.", 80, 4),
            [SystemRoles.Doctor] = ("Bác sĩ", "Khám bệnh, xem lịch hẹn.", 50, 2),
            [SystemRoles.Receptionist] = ("Lễ tân", "Đặt lịch, check-in, quản lý hồ sơ BN.", 30, 3),
            [SystemRoles.Patient] = ("Bệnh nhân", "Đặt lịch, xem hồ sơ cá nhân.", 10, 1),
        };

        public static async Task SeedAsync(
            ClinicDbContext context,
            CancellationToken cancellationToken = default)
        {
            // 1. Lấy các role constant
            var roleFields = typeof(SystemRoles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            // 2. Lấy code đã có trong DB
            var existingCodes = await context.Roles
                .Select(r => r.Code)
                .ToListAsync(cancellationToken);
            var existingSet = existingCodes.ToHashSet();

            // 3. Tạo mới role chưa có
            var toAdd = new List<Role>();

            foreach (var field in roleFields)
            {
                var code = (string)field.GetRawConstantValue()!;

                if (existingSet.Contains(code))
                    continue;

                if (!RoleMetadata.TryGetValue(code, out var meta))
                    continue;

                var role = new Role(
                    code: code,
                    name: meta.Name,
                    description: meta.Description,
                    displayOrder: meta.DisplayOrder,    // ← Tách riêng
                    priority: meta.Priority,            // ← Tách riêng
                    isSystemRole: true);

                toAdd.Add(role);
            }

            if (toAdd.Any())
            {
                context.Roles.AddRange(toAdd);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}