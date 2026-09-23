using System;

namespace BaseClinic.Domain.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        public Role Role { get; private set; } = null!;
        public Permission Permission { get; private set; } = null!;

        private RolePermission() { } // Dành cho ORM

        // Internal để bắt buộc thao tác qua method của đối tượng Role
        internal RolePermission(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}