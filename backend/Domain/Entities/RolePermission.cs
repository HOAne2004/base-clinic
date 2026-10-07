using BaseClinic.Domain.Common;

namespace BaseClinic.Domain.Entities
{
    public class RolePermission : Entity
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        public Role Role { get; private set; } = null!;
        public Permission Permission { get; private set; } = null!;

        /// <summary>Ai gán permission này cho role.</summary>
        public Guid? GrantedBy { get; private set; }

        /// <summary>Khi nào gán.</summary>
        public DateTimeOffset GrantedAt { get; private set; }

        private RolePermission() { } // Dành cho ORM

        // Internal để bắt buộc thao tác qua method của đối tượng Role
        internal RolePermission(Guid roleId, Guid permissionId, Guid grantedBy)
        {
            RoleId = roleId;
            PermissionId = permissionId;
            GrantedBy = grantedBy;
            GrantedAt = DateTimeOffset.UtcNow;
        }
    }
}