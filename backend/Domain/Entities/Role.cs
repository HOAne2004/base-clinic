using BaseClinic.Domain.Common;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    public class Role : AuditableEntity
    {
        //  Mã định danh — "ADMIN"
        public string Code { get; protected set; } = string.Empty; // string.Empty khởi tạo chuỗi rỗng "" | null! khởi tạo null
        
        // Tên hiển thị - "Quản trị viên"
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        // Thứ tự hiển thị
        public int DisplayOrder { get; private set; }

        // Độ ưu tiên — số lớn hơn = ưu tiên cao hơn.
        public int Priority { get; private set; }
        public bool IsSystemRole { get; private set; } // Bảo vệ role của hệ thống


        //Còn dùng không.
        public bool IsActive { get; private set; } = true;

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();


        private readonly List<AccountRole> _accountRoles = new();
        public IReadOnlyCollection<AccountRole> AccountRoles => _accountRoles.AsReadOnly();
        private Role() { } // Dành cho ORM (EF Core)

        public Role(
            string code,
            string name,
            string? description = null,
            int displayOrder = 0,
            int priority = 0,
            bool isSystemRole = false)
        {
            SetCode(code);
            SetName(name);
            SetDescription(description);
            SetDisplayOrder(displayOrder);

            Priority = priority;
            IsSystemRole = isSystemRole;
            IsActive = true;
        }

        // ==================== UPDATE METHODS ====================
        public void SetCode(string code)
        {
            if (IsSystemRole)
                throw new DomainException("Không thể đổi mã của System Role.");
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Mã role không được để trống.");
            if (code.Length > 50)
                throw new DomainException("Mã role không vượt quá 50 ký tự.");

            Code = code.Trim().ToUpperInvariant();
        }

        public void SetName(string name)
        {
            if (IsSystemRole)
                throw new DomainException("Không thể đổi tên của System Role.");
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên role không được để trống.");
            if (name.Length > 100)
                throw new DomainException("Tên role không vượt quá 100 ký tự.");

            Name = name.Trim();
        }

        public void SetDescription(string? description)
        {
            if (IsSystemRole)
                throw new DomainException("Không thể đổi mô tả của System Role.");
            if (description?.Length > 500)
                throw new DomainException("Mô tả role không vượt quá 500 ký tự.");

            Description = description?.Trim();
        }

        public void SetDisplayOrder(int order)
        {
            if (order < 0)
                throw new DomainException("Thứ tự hiển thị không được âm.");
            DisplayOrder = order;
        }

        public void SetPriority(int priority) => Priority = priority;

        public void Activate() => IsActive = true;

        public void Deactivate()
        {
            if (IsSystemRole)
                throw new DomainException("Không thể vô hiệu hóa System Role.");
            IsActive = false;
        }

        // ==================== PERMISSION MANAGEMENT ====================
        /// <summary>Gán permission cho role. Idempotent — gán trùng không lỗi.</summary>
        public void AssignPermission(Guid permissionId, Guid grantedBy)
        {
            if (_rolePermissions.Any(x => x.PermissionId == permissionId))
                return;

            _rolePermissions.Add(new RolePermission(Id, permissionId, grantedBy));
        }

        /// <summary>Gỡ permission khỏi role.</summary>
        public void RemovePermission(Guid permissionId)
        {
            var perm = _rolePermissions.FirstOrDefault(x => x.PermissionId == permissionId);
            if (perm != null)
                _rolePermissions.Remove(perm);
        }

        /// <summary>
        /// Đồng bộ toàn bộ permission của role với danh sách mới.
        /// Dùng khi UI gửi mảng checkbox — thay vì gọi Assign/Remove từng cái.
        /// </summary>
        public void SyncPermissions(IEnumerable<Guid> permissionIds, Guid grantedBy)
        {
            var targetIds = permissionIds.ToHashSet();
            var currentIds = _rolePermissions.Select(x => x.PermissionId).ToHashSet();

            // Xóa những permission không còn trong danh sách mới
            var toRemove = _rolePermissions
                .Where(x => !targetIds.Contains(x.PermissionId))
                .ToList();
            foreach (var item in toRemove)
                _rolePermissions.Remove(item);

            // Thêm những permission mới
            foreach (var permId in targetIds.Except(currentIds))
                _rolePermissions.Add(new RolePermission(Id, permId, grantedBy));
        }
    }

}