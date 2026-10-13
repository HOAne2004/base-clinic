using BaseClinic.Domain.Common;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Domain.Helpers;
using System;
using System.Text.RegularExpressions;

namespace BaseClinic.Domain.Entities
{
    public class Department : FullAuditedEntity
    {
        // ==================== HẰNG SỐ MẶC ĐỊNH ====================
        public static readonly TimeOnly DefaultOpeningTime = new TimeOnly(7, 0);
        public static readonly TimeOnly DefaultClosingTime = new TimeOnly(17, 0);
        public const int DefaultSortOrder = 999;
        public const int MaxSlugLength = 200;

        // ==================== PROPERTIES ====================
        public string Name { get; private set; } = null!;
        public string DepartmentCode { get; private set; } = null!;

        // Auto-generate từ Name, Admin có thể override.
        public string Slug { get; private set; } = string.Empty;
        public string? ImageUrl { get; private set; }
        public string? ShortDescription { get; private set; }
        public string? FullDescription { get; private set; }
        public string? Location { get; private set; }
        public string? PhoneNumber { get; private set; }

        /// <summary>
        /// Khoa có mở cửa 24/24 không.
        /// - true: Mở liên tục, bỏ qua OpeningTime/ClosingTime.
        /// - false: Chỉ mở trong khoảng OpeningTime - ClosingTime.
        /// </summary>
        public bool Is24Hours { get; private set; }

        /// <summary>Giờ mở cửa hàng ngày. Chỉ có ý nghĩa khi Is24Hours = false.</summary>
        public TimeOnly OpeningTime { get; private set; }

        /// <summary>Giờ đóng cửa hàng ngày. Chỉ có ý nghĩa khi Is24Hours = false.</summary>
        public TimeOnly ClosingTime { get; private set; }

        /// <summary>
        /// Thứ tự sắp xếp khi hiển thị danh sách.
        /// Số nhỏ hiển thị trước. Min = 1. Default = 999.
        /// </summary>
        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; } = true;

        // ==================== CONSTRUCTOR ====================
        private Department() { } // EF Core

        public Department(
            string name,
            string departmentCode,
            string? slug,
            string? imageUrl,
            string? shortDescription,
            string? fullDescription,
            string? location,
            string? phoneNumber,
            bool is24Hours = false,
            TimeOnly? openingTime = null,
            TimeOnly? closingTime = null,
            int sortOrder = DefaultSortOrder)
        {
            ValidateCode(departmentCode);
            DepartmentCode = departmentCode.Trim().ToUpperInvariant();

            UpdateName(name);
            UpdateSlug(slug ?? SlugHelper.Generate(name));
            UpdateDetails(imageUrl, shortDescription, fullDescription, location, phoneNumber);
            UpdateOperatingHours(
                is24Hours,
                openingTime ?? DefaultOpeningTime,
                closingTime ?? DefaultClosingTime);
            SetSortOrder(sortOrder);
            Activate();
        }

        // ==================== UPDATE METHODS ====================
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên khoa không được để trống.", nameof(name));
            if (name.Length > 100)
                throw new ArgumentException("Tên khoa không được vượt quá 100 ký tự.", nameof(name));

            Name = name.Trim();
        }

        /// <summary>
        /// Cập nhật slug. Admin có thể gọi để override.
        /// Nếu truyền chuỗi rỗng → auto-generate từ Name hiện tại.
        /// </summary>
        public void UpdateSlug(string? slug)
        {
            var finalSlug = string.IsNullOrWhiteSpace(slug)
                ? SlugHelper.Generate(Name)
                : SlugHelper.Generate(slug);

            if (string.IsNullOrEmpty(finalSlug))
                throw new DomainException("Không thể sinh slug từ tên khoa.");

            if (finalSlug.Length > MaxSlugLength)
                throw new DomainException($"Slug không vượt quá {MaxSlugLength} ký tự.");

            Slug = finalSlug;
        }

        public void UpdateDetails(
            string? imageUrl,
            string? shortDescription,
            string? fullDescription,
            string? location,
            string? phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber) && !Regex.IsMatch(phoneNumber, @"^\d{8,15}$"))
                throw new ArgumentException("Số điện thoại khoa không hợp lệ.", nameof(phoneNumber));

            ImageUrl = imageUrl?.Trim();
            ShortDescription = shortDescription?.Trim();
            FullDescription = fullDescription?.Trim();
            Location = location?.Trim();
            PhoneNumber = phoneNumber?.Trim();
        }

        /// <summary>
        /// Cập nhật giờ hoạt động.
        /// 
        /// - Nếu is24Hours = true → bỏ qua check opening/closing.
        /// - Nếu is24Hours = false → opening phải < closing.
        /// </summary>
        public void UpdateOperatingHours(
            bool is24Hours,
            TimeOnly openingTime,
            TimeOnly closingTime)
        {
            if (!is24Hours && openingTime >= closingTime)
                throw new ArgumentException(
                    "Giờ mở cửa phải trước giờ đóng cửa (hoặc chọn chế độ 24/24).",
                    nameof(openingTime));

            Is24Hours = is24Hours;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
        }

        /// <summary>
        /// Đặt chế độ 24/24 — bỏ qua giờ mở/đóng.
        /// </summary>
        public void Set24HoursMode() => Is24Hours = true;

        /// <summary>
        /// Đặt chế độ theo giờ hành chính (mặc định 07:00 - 17:00).
        /// </summary>
        public void SetOperatingHoursMode(
            TimeOnly? openingTime = null,
            TimeOnly? closingTime = null)
        {
            var open = openingTime ?? DefaultOpeningTime;
            var close = closingTime ?? DefaultClosingTime;

            UpdateOperatingHours(false, open, close);
        }

        /// <summary>
        /// Cập nhật thứ tự sắp xếp hiển thị. Min = 1.
        /// </summary>
        public void SetSortOrder(int sortOrder)
        {
            if (sortOrder < 1)
                throw new ArgumentException(
                    "Thứ tự sắp xếp phải >= 1.",
                    nameof(sortOrder));

            SortOrder = sortOrder;
        }

        // ==================== STATUS ====================
        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        // ==================== VALIDATION ====================
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