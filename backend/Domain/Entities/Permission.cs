using BaseClinic.Domain.Common;
using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.Entities
{
    public class Permission : AuditableEntity
    {
        // "patient:read"
        public string Code { get; protected set; } = string.Empty;

        // "Xem bệnh nhân"
        public string Name { get; private set; } = null!;

        // "Patient"
        public string Resource { get; private set; } = null!;

        // "Read"
        public string Action { get; private set; } = null!;

        // Mô tả chi tiết
        public string? Description { get; private set; }

        // Thứ tự hiển thị trên UI
        public int DisplayOrder { get; private set; }

        // Navigation
        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Permission() { }

        public Permission(
            string code,
            string name,
            string resource,
            string action,
            string? description = null,
            int displayOrder = 0)
        {
            SetCode(code);
            SetName(name);
            SetResource(resource);
            SetAction(action);
            SetDescription(description);

            if (displayOrder < 0)
                throw new DomainException("Thứ tự hiển thị không được âm.");
            DisplayOrder = displayOrder;
        }

        private void SetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Mã permission không được để trống.");
            if (code.Length > 100)
                throw new DomainException("Mã permission không vượt quá 100 ký tự.");

            Code = code.Trim().ToLowerInvariant();
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên permission không được để trống.");
            if (name.Length > 200)
                throw new DomainException("Tên permission không vượt quá 200 ký tự.");

            Name = name.Trim();
        }

        private void SetResource(string resource)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new DomainException("Resource không được để trống.");
            if (resource.Length > 100)
                throw new DomainException("Resource không vượt quá 100 ký tự.");

            Resource = resource.Trim();
        }

        private void SetAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new DomainException("Action không được để trống.");
            if (action.Length > 50)
                throw new DomainException("Action không vượt quá 50 ký tự.");

            Action = action.Trim();
        }

        private void SetDescription(string? description)
        {
            if (description?.Length > 500)
                throw new DomainException("Mô tả không vượt quá 500 ký tự.");

            Description = description?.Trim();
        }
    }
}