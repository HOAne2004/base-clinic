using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Domain.Entities
{
    public class Account : AggregateRoot
    {
        public string FullName { get; private set; } = string.Empty;
        public string? Email { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public AccountStatus Status { get; private set; }
        public bool IsEmailVerified { get; private set; } = false;
        public bool IsPhoneNumberVerified { get; private set; } = false;

        // Tracking & Security
        public DateTimeOffset? LastLoginAt { get; private set; }
        public DateTimeOffset? LastPasswordChangedAt { get; private set; }
        public int FailedLoginAttempts { get; private set; } = 0;
        public DateTimeOffset? LockoutEnd { get; private set; }

        // Navigation property (Chỉ cho phép đọc từ bên ngoài)
        private readonly List<AccountRole> _accountRoles = new();
        public IReadOnlyCollection<AccountRole> AccountRoles => _accountRoles.AsReadOnly();

        private Account() { } // Dành cho ORM (EF Core)

        public Account(string fullName, string? email, string passwordHash, string phoneNumber, AccountStatus status, bool isEmailVerified, bool isPhoneNumberVerified)
        {
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            PhoneNumber = phoneNumber;
            Status = status;
            IsEmailVerified = isEmailVerified;
            IsPhoneNumberVerified = isPhoneNumberVerified;
        }

        public void ChangeFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Tên không được để trống.", nameof(fullName));
            FullName = fullName.Trim();
        }

        public void ChangeEmail(string? email)
        {
            var newEmail = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            if (Email != newEmail)
            {
                Email = newEmail;
                IsEmailVerified = false; // Reset cờ xác thực khi đổi email
            }
        }

        public void ChangePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Số điện thoại không được để trống.", nameof(phoneNumber));

            var newPhone = phoneNumber.Trim();
            if (PhoneNumber != newPhone)
            {
                PhoneNumber = newPhone;
                IsPhoneNumberVerified = false; // Reset cờ xác thực khi đổi SĐT
            }
        }

        public void ChangePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Mật khẩu không được để trống.", nameof(passwordHash));

            PasswordHash = passwordHash;
            LastPasswordChangedAt = DateTimeOffset.UtcNow; // Ghi vết thời điểm đổi mật khẩu
        }

        public void VerifyEmail() => IsEmailVerified = true;
        public void VerifyPhoneNumber() => IsPhoneNumberVerified = true;

        public void Activate() => Status = AccountStatus.Active;
        public void Deactivate() => Status = AccountStatus.Inactive;
        public void Suspend() => Status = AccountStatus.Suspended;

        // Security / Login Behaviors
        public void RecordLogin(DateTimeOffset loginAt)
        {
            LastLoginAt = loginAt;
            FailedLoginAttempts = 0; // Reset số lần thử nếu đăng nhập thành công
        }

        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;
        }

        public void LockAccount(DateTimeOffset lockoutEnd)
        {
            LockoutEnd = lockoutEnd;
        }

        public void UnlockAccount()
        {
            LockoutEnd = null;
            FailedLoginAttempts = 0;
        }

        // --- ROLE MANAGEMENT METHODS ---
        public void AssignRole(Guid roleId)
        {
            if (!_accountRoles.Any(x => x.RoleId == roleId))
            {
                _accountRoles.Add(new AccountRole(Id, roleId));
            }
        }

        public void RemoveRole(Guid roleId)
        {
            var role = _accountRoles.FirstOrDefault(x => x.RoleId == roleId);
            if (role != null)
            {
                _accountRoles.Remove(role);
            }
        }
    }
}