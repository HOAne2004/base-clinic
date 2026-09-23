using BaseClinic.Domain.Common;
using System;

namespace BaseClinic.Domain.Entities
{
    public class Department : AggregateRoot
    {
        public string Name { get; private set; } = null!;
        public string DepartmentCode { get; private set; } = null!;
        public string? ImageUrl { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Department() { } // Dành cho ORM

        public Department(string name, string departmentCode, string? imageUrl, string? description)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
                throw new ArgumentException("Mã khoa không được để trống.", nameof(departmentCode));

            if (departmentCode.Length > 20)
                throw new ArgumentException("Mã khoa không được vượt quá 20 ký tự.", nameof(departmentCode));

            DepartmentCode = departmentCode.Trim(); // Immutable sau khi khởi tạo

            UpdateName(name);
            UpdateDetails(imageUrl, description);
            Activate();
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên khoa không được để trống.", nameof(name));

            if (name.Length > 100)
                throw new ArgumentException("Tên khoa không được vượt quá 100 ký tự.", nameof(name));

            Name = name.Trim();
        }

        public void UpdateDetails(string? imageUrl, string? description)
        {
            ImageUrl = imageUrl?.Trim();
            Description = description?.Trim();
        }

        // Sử dụng Ubiquitous Language thay vì Set/Update IsActive
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}