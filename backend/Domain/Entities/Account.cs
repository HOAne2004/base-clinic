using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Tài khoản người dùng — đại diện danh tính đăng nhập.
    /// Rich Domain Model: mọi thay đổi trạng thái phải đi qua method.
    /// </summary>
    public class Account : FullAuditedEntity
    {
        // ==================== IDENTITY ====================
        /// <summary>Tên pháp lý (theo CCCD).</summary>
        public string FullName { get; private set; } = string.Empty;

        /// <summary>Email — optional, dùng để nhận thông báo.</summary>
        public string? Email { get; private set; }

        /// <summary>Số điện thoại — dùng để đăng nhập.</summary>
        public string PhoneNumber { get; private set; } = string.Empty;

        /// <summary>Mật khẩu đã hash.</summary>
        public string PasswordHash { get; private set; } = string.Empty;

        // ==================== PROFILE ====================
        public string? AvatarUrl { get; private set; }
        public string PreferredLanguage { get; private set; } = "vi";

        // ==================== STATUS ====================
        public AccountStatus Status { get; private set; }
        public bool IsEmailVerified { get; private set; }
        public bool IsPhoneNumberVerified { get; private set; }

        // ==================== SECURITY ====================
        public DateTimeOffset? LastLoginAt { get; private set; }
        public DateTimeOffset? LastPasswordChangedAt { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public DateTimeOffset? LockoutEnd { get; private set; }
        public bool TwoFactorEnabled { get; private set; }

        /// <summary>
        /// Security Stamp — đổi khi cần invalidate mọi token hiện có.
        /// So sánh với claim trong JWT mỗi request để phát hiện token cũ.
        /// </summary>
        public Guid SecurityStamp { get; private set; }

        /// <summary>
        /// Cờ bắt buộc đổi mật khẩu ở lần đăng nhập kế tiếp.
        /// Set = true khi Admin reset mật khẩu.
        /// </summary>
        public bool MustChangePasswordOnNextLogin { get; private set; }

        // ==================== DEACTIVATION ====================
        public DateTimeOffset? DeactivatedAt { get; private set; }
        public Guid? DeactivatedBy { get; private set; }
        public string? DeactivationReason { get; private set; }

        // ==================== HẰNG SỐ NỘI BỘ ====================
        /// <summary>Số lần nhập sai tối đa trước khi khóa tài khoản.</summary>
        private const int MaxFailedLoginAttempts = 5;

        /// <summary>Thời gian khóa tài khoản (phút) khi vượt ngưỡng.</summary>
        private const int LockoutDurationMinutes = 15;

        /// <summary>Số thiết bị đăng nhập tối đa cùng lúc.</summary>
        private const int MaxActiveDevices = 2;

        // ==================== NAVIGATION ====================
        private readonly List<AccountRole> _accountRoles = new();
        public IReadOnlyCollection<AccountRole> AccountRoles => _accountRoles.AsReadOnly();

        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        private readonly List<LoginHistory> _loginHistories = new();
        public IReadOnlyCollection<LoginHistory> LoginHistories => _loginHistories.AsReadOnly();

        private readonly List<OtpCode> _otpCodes = new();
        public IReadOnlyCollection<OtpCode> OtpCodes => _otpCodes.AsReadOnly();

        private Account() { } // EF Core

        /// <summary>
        /// Tạo tài khoản mới. Trạng thái mặc định là Inactive — chờ xác thực OTP.
        /// </summary>
        public Account(
            string fullName,
            string phoneNumber,
            string passwordHash,
            string? email = null)
        {
            SetFullName(fullName);
            SetPhoneNumber(phoneNumber);
            SetPasswordHash(passwordHash);
            SetEmail(email);

            Status = AccountStatus.Inactive;
            FailedLoginAttempts = 0;
            TwoFactorEnabled = false;
            IsEmailVerified = false;
            IsPhoneNumberVerified = false;
            MustChangePasswordOnNextLogin = false;
            SecurityStamp = Guid.NewGuid();
        }

        // ==================== PROFILE METHODS ====================
        public void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Họ tên không được để trống.");
            if (fullName.Length > 100)
                throw new DomainException("Họ tên không vượt quá 100 ký tự.");

            FullName = fullName.Trim();
        }

        /// <summary>Đổi email. Reset cờ xác thực nếu email thay đổi.</summary>
        public void SetEmail(string? email)
        {
            var normalized = string.IsNullOrWhiteSpace(email)
                ? null
                : email.Trim().ToLowerInvariant();

            if (normalized != null && normalized.Length > 255)
                throw new DomainException("Email không vượt quá 255 ký tự.");

            if (Email != normalized)
            {
                Email = normalized;
                IsEmailVerified = false;
            }
        }

        /// <summary>Đổi SĐT. Reset cờ xác thực nếu SĐT thay đổi.</summary>
        public void SetPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Số điện thoại không được để trống.");
            if (phoneNumber.Length > 20)
                throw new DomainException("Số điện thoại không vượt quá 20 ký tự.");

            var normalized = phoneNumber.Trim();
            if (PhoneNumber != normalized)
            {
                PhoneNumber = normalized;
                IsPhoneNumberVerified = false;
            }
        }

        public void UpdateAvatar(string? avatarUrl)
            => AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();

        public void SetPreferredLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new DomainException("Ngôn ngữ không được để trống.");

            PreferredLanguage = language.Trim();
        }

        // ==================== PASSWORD ====================
        /// <summary>
        /// Gán mật khẩu hash. Private — chỉ dùng nội bộ trong class.
        /// Set LastPasswordChangedAt = now.
        /// </summary>
        private void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Mật khẩu không được để trống.");

            PasswordHash = passwordHash;
            LastPasswordChangedAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// User tự đổi mật khẩu (qua chức năng "Đổi mật khẩu" hoặc "Quên mật khẩu").
        /// Tự động:
        /// - Revoke tất cả refresh token (buộc login lại các thiết bị khác).
        /// - Unlock tài khoản nếu đang bị khóa do nhập sai.
        /// - Clear cờ MustChangePasswordOnNextLogin.
        /// </summary>
        public void ChangePassword(string newPasswordHash)
        {
            SetPasswordHash(newPasswordHash);
            SecurityStamp = Guid.NewGuid();
            MustChangePasswordOnNextLogin = false;
            RevokeAllRefreshTokens("Đổi mật khẩu — yêu cầu đăng nhập lại.");
            UnlockAccount();

        }

        /// <summary>
        /// Admin reset mật khẩu. User bắt buộc đổi ở lần đăng nhập kế tiếp.
        /// Tự động:
        /// - Set MustChangePasswordOnNextLogin = true.
        /// - Revoke tất cả refresh token.
        /// - Unlock tài khoản (admin reset thì coi như xóa lịch sử nhập sai).
        /// </summary>
        public void AdminResetPassword(string tempPasswordHash)
        {
            SetPasswordHash(tempPasswordHash);
            SecurityStamp = Guid.NewGuid();
            MustChangePasswordOnNextLogin = true;
            RevokeAllRefreshTokens("Admin reset mật khẩu.");
            UnlockAccount();
        }

        /// <summary>
        /// User hoàn tất đổi mật khẩu sau khi được Admin reset.
        /// Clear cờ MustChangePasswordOnNextLogin.
        /// </summary>
        public void CompleteForcedPasswordChange(string newPasswordHash)
        {
            SetPasswordHash(newPasswordHash);
            SecurityStamp = Guid.NewGuid();
            MustChangePasswordOnNextLogin = false;
            RevokeAllRefreshTokens("Phiên bản đăng nhập hết hạn.");
        }

        // ==================== VERIFICATION ====================
        public void VerifyEmail() => IsEmailVerified = true;
        public void VerifyPhoneNumber() => IsPhoneNumberVerified = true;

        // ==================== STATUS MANAGEMENT ====================
        public void Activate()
        {
            if (Status == AccountStatus.Suspended)
                throw new DomainException("Không thể kích hoạt tài khoản đang bị suspend. Vui lòng unlock trước.");

            Status = AccountStatus.Active;
        }

        public void Suspend(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Lý do suspend không được để trống.");

            Status = AccountStatus.Suspended;
            SecurityStamp = Guid.NewGuid();          
            RevokeAllRefreshTokens("Tài khoản bị tạm ngưng.");
        }

        /// <summary>
        /// Vô hiệu hóa tài khoản (do admin). Khác với Suspend (khóa tạm).
        /// </summary>
        public void Deactivate(string reason, Guid deactivatedBy)
        {
            if (Status == AccountStatus.Suspended)
                throw new DomainException("Không thể deactivate tài khoản đang bị suspend. Vui lòng unlock trước.");

            Status = AccountStatus.Inactive;
            DeactivatedAt = DateTimeOffset.UtcNow;
            DeactivatedBy = deactivatedBy;
            DeactivationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            SecurityStamp = Guid.NewGuid();
            RevokeAllRefreshTokens("Tài khoản bị vô hiệu hóa.");
        }

        /// <summary>Khôi phục tài khoản đã deactivate.</summary>
        public void RestoreFromDeactivation()
        {
            Status = AccountStatus.Active;
            DeactivatedAt = null;
            DeactivatedBy = null;
            DeactivationReason = null;
        }

        // ==================== LOGIN / SECURITY ====================

        /// <summary>
        /// Kiểm tra tài khoản có đang bị khóa tạm thời do nhập sai quá nhiều lần không.
        /// </summary>
        public bool IsLockedOut()
            => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;

        /// <summary>
        /// Ghi nhận đăng nhập thành công. Reset số lần nhập sai.
        /// </summary>
        public void RecordLogin(DateTimeOffset loginAt, string? ipAddress, string? deviceInfo)
        {
            LastLoginAt = loginAt;
            FailedLoginAttempts = 0;
            LockoutEnd = null;

            _loginHistories.Add(LoginHistory.Success(Id, loginAt, ipAddress, deviceInfo));
        }

        /// <summary>
        /// Ghi nhận đăng nhập thất bại.
        /// Tự động khóa tài khoản nếu số lần sai đạt ngưỡng (5 lần → khóa 15 phút).
        /// </summary>
        public void RecordFailedLogin(string? ipAddress, string? deviceInfo, string? reason)
        {
            FailedLoginAttempts++;

            _loginHistories.Add(LoginHistory.Failed(
                Id, DateTimeOffset.UtcNow, ipAddress, deviceInfo, reason));

            if (FailedLoginAttempts >= MaxFailedLoginAttempts)
            {
                LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(LockoutDurationMinutes);
            }
        }

        /// <summary>Khóa tài khoản thủ công (bởi admin).</summary>
        public void LockAccount(DateTimeOffset lockoutEnd)
        {
            if (lockoutEnd <= DateTimeOffset.UtcNow)
                throw new DomainException("Thời gian khóa phải ở tương lai.");

            LockoutEnd = lockoutEnd;
        }

        /// <summary>Mở khóa tài khoản. Reset cả số lần nhập sai.</summary>
        public void UnlockAccount()
        {
            LockoutEnd = null;
            FailedLoginAttempts = 0;
        }

        public void EnableTwoFactor() => TwoFactorEnabled = true;
        public void DisableTwoFactor() => TwoFactorEnabled = false;

        /// <summary>
        /// Đổi SecurityStamp + revoke toàn bộ refresh token.
        /// Dùng khi user chọn "Đăng xuất khỏi mọi thiết bị".
        /// </summary>
        public void LogoutAllDevices()
        {
            SecurityStamp = Guid.NewGuid();
            RevokeAllRefreshTokens("Đăng xuất khỏi mọi thiết bị.");
        }

        // ==================== ROLE MANAGEMENT ====================
        /// <summary>Gán role. Idempotent — gán trùng không lỗi.</summary>
        public void AssignRole(Guid roleId, Guid? assignedBy = null)
        {
            if (_accountRoles.Any(x => x.RoleId == roleId))
                return;

            _accountRoles.Add(new AccountRole(Id, roleId, assignedBy));
        }

        public void RemoveRole(Guid roleId)
        {
            var role = _accountRoles.FirstOrDefault(x => x.RoleId == roleId);
            if (role != null)
                _accountRoles.Remove(role);
        }

        public void ClearRoles() => _accountRoles.Clear();

        // ==================== REFRESH TOKEN ====================
        /// <summary>
        /// Thêm refresh token mới. Nếu vượt quá MaxActiveDevices,
        /// tự động revoke token cũ nhất.
        /// </summary>
        public void AddRefreshToken(RefreshToken token)
        {
            if (token == null)
                throw new DomainException("Token không được null.");

            var activeTokens = _refreshTokens
                .Where(t => t.IsActive)
                .OrderBy(t => t.CreatedAt)
                .ToList();

            if (activeTokens.Count >= MaxActiveDevices)
            {
                var oldest = activeTokens.First();
                oldest.Revoke("Vượt quá số lượng thiết bị cho phép.");
            }

            _refreshTokens.Add(token);
        }

        /// <summary>
        /// Revoke tất cả refresh token — dùng khi đổi mật khẩu, logout all devices,
        /// hoặc admin reset password.
        /// </summary>
        public void RevokeAllRefreshTokens(string reason)
        {
            foreach (var token in _refreshTokens.Where(t => t.IsActive))
                token.Revoke(reason);
        }
    }
}