using BaseClinic.Domain.Common;
using System;
using System.Text.RegularExpressions;

namespace BaseClinic.Domain.Entities
{
    public class Department : FullAuditedEntity
    {
        public string Name { get; private set; } = null!;
        public string DepartmentCode { get; private set; } = null!;
        public string? ImageUrl { get; private set; }
        public string? ShortDescription { get; private set; }   // dùng cho list
        public string? FullDescription { get; private set; }    // dùng cho detail
        public string? Location { get; private set; }           // THÊM: tầng/phòng
        public string? PhoneNumber { get; private set; }        // THÊM: số nội bộ
        public bool IsActive { get; private set; } = true;

        private Department() { }

        public Department(string name, string departmentCode, string? imageUrl,
                          string? shortDescription, string? fullDescription,
                          string? location, string? phoneNumber)
        {
            ValidateCode(departmentCode);
            DepartmentCode = departmentCode.Trim().ToUpperInvariant();

            UpdateName(name);
            UpdateDetails(imageUrl, shortDescription, fullDescription, location, phoneNumber);
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

        public void UpdateDetails(string? imageUrl, string? shortDescription,
                                  string? fullDescription, string? location, string? phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber) && !Regex.IsMatch(phoneNumber, @"^\d{8,15}$"))
                throw new ArgumentException("Số điện thoại khoa không hợp lệ.", nameof(phoneNumber));

            ImageUrl = imageUrl?.Trim();
            ShortDescription = shortDescription?.Trim();
            FullDescription = fullDescription?.Trim();
            Location = location?.Trim();
            PhoneNumber = phoneNumber?.Trim();
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        private static void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Mã khoa không được để trống.", nameof(code));
            if (code.Length > 20)
                throw new ArgumentException("Mã khoa không được vượt quá 20 ký tự.", nameof(code));
            if (!Regex.IsMatch(code, @"^[A-Za-z0-9_]{2,20}$"))
                throw new ArgumentException("Mã khoa chỉ được chứa chữ, số và dấu gạch dưới.", nameof(code));
        }
    }
}