using BaseClinic.Domain.Common;

namespace BaseClinic.Domain.Entities
{
    public class Role : AuditableEntity
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public bool IsSystemRole { get; private set; } // Bảo vệ role của hệ thống

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Role() { } // Dành cho ORM (EF Core)

        public Role(string name, string description, bool isSystemRole = false)
        {
            Name = name;
            Description = description;
            IsSystemRole = isSystemRole;
        }

        public void Update(string name, string? description)
        {
            if (IsSystemRole)
            {
                throw new InvalidOperationException("Không được phép thay đổi thông tin của System Role (ví dụ: Admin, Doctor).");
            }

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role không được để trống.", nameof(name));

            if (name.Length > 50)
                throw new ArgumentException("Tên Role không được vượt quá 50 ký tự.", nameof(name));

            if (description?.Length > 200)
                throw new ArgumentException("Mô tả Role không được vượt quá 200 ký tự.", nameof(description));

            Name = name.Trim();
            Description = description?.Trim();
        }

        // --- PERMISSION MANAGEMENT METHODS ---
        public void AssignPermission(Guid permissionId)
        {
            if (!_rolePermissions.Any(x => x.PermissionId == permissionId))
            {
                _rolePermissions.Add(new RolePermission(Id, permissionId));
            }
        }

        public void RemovePermission(Guid permissionId)
        {
            var permission = _rolePermissions.FirstOrDefault(x => x.PermissionId == permissionId);
            if (permission != null)
            {
                _rolePermissions.Remove(permission);
            }
        }
    }
}