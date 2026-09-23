using System;

namespace BaseClinic.Domain.Entities
{
    public class AccountRole
    {
        public Guid AccountId { get; private set; }
        public Guid RoleId { get; private set; }

        public Account Account { get; private set; } = null!;
        public Role Role { get; private set; } = null!;

        private AccountRole() { } // Dành cho ORM

        // Internal để bắt buộc thao tác qua method của đối tượng Account
        internal AccountRole(Guid accountId, Guid roleId)
        {
            AccountId = accountId;
            RoleId = roleId;
        }
    }
}