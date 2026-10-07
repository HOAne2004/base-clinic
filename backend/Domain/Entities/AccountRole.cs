using BaseClinic.Domain.Common;

namespace BaseClinic.Domain.Entities
{
    public class AccountRole : Entity
    {
        public Guid AccountId { get; private set; }
        public Guid RoleId { get; private set; }

        public Account Account { get; private set; } = null!;
        public Role Role { get; private set; } = null!;

        /// <summary>Ai gán role này cho account.</summary>
        public Guid? AssignedBy { get; private set; }

        /// <summary>Khi nào gán.</summary>
        public DateTimeOffset AssignedAt { get; private set; }

        private AccountRole() { } // Dành cho ORM

        // Internal để bắt buộc thao tác qua method của đối tượng Account
        internal AccountRole(Guid accountId, Guid roleId, Guid? assignedBy = null)
        {
            AccountId = accountId;
            RoleId = roleId;
            AssignedBy = assignedBy;
            AssignedAt = DateTimeOffset.UtcNow;
        }
    }
}