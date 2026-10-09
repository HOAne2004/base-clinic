using BaseClinic.Domain.Exceptions;

namespace BaseClinic.Domain.ValueObjects
{
    /// <summary>
    /// Value Object đại diện cho địa chỉ (theo địa giới hành chính VN: Tỉnh → Xã).
    /// 
    /// ĐẶC ĐIỂM:
    /// - Immutable: mọi thay đổi phải tạo instance mới.
    /// - Equality by value: 2 Address bằng nhau nếu mọi field bằng nhau.
    /// - Không có Id: là Value Object, không phải Entity.
    /// - Được lưu dưới dạng Owned Entity (cột cùng bảng cha).
    /// </summary>
    public sealed class Address : IEquatable<Address>
    {
        public string Tinh { get; private set; } = string.Empty;
        public string Xa { get; private set; } = string.Empty;
        public string Duong { get; private set; } = string.Empty;
        public string SoNha { get; private set; } = string.Empty;

        /// <summary>Chuỗi địa chỉ đầy đủ — computed, không lưu DB.</summary>
        public string FullText => BuildFullText();

        /// <summary>Instance rỗng — dùng làm default cho EF Core.</summary>
        public static Address Empty => new Address(string.Empty, string.Empty, string.Empty, string.Empty);

        private Address() { } // EF Core

        public Address(string tinh, string xa, string duong, string soNha)
        {
            // Cho phép rỗng (khi Patient chưa có địa chỉ)
            // Nhưng nếu có bất kỳ field nào thì phải hợp lệ
            SetTinh(tinh);
            SetXa(xa);
            SetDuong(duong);
            SetSoNha(soNha);
        }

        private void SetTinh(string tinh)
        {
            if (tinh?.Length > 100)
                throw new DomainException("Tỉnh/Thành phố không vượt quá 100 ký tự.");
            Tinh = tinh?.Trim() ?? string.Empty;
        }

        private void SetXa(string xa)
        {
            if (xa?.Length > 100)
                throw new DomainException("Xã/Phường không vượt quá 100 ký tự.");
            Xa = xa?.Trim() ?? string.Empty;
        }

        private void SetDuong(string duong)
        {
            if (duong?.Length > 200)
                throw new DomainException("Đường không vượt quá 200 ký tự.");
            Duong = duong?.Trim() ?? string.Empty;
        }

        private void SetSoNha(string soNha)
        {
            if (soNha?.Length > 50)
                throw new DomainException("Số nhà không vượt quá 50 ký tự.");
            SoNha = soNha?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Xây dựng chuỗi địa chỉ đầy đủ.
        /// Format: "Số nhà Đường, Xã, Tỉnh"
        /// Ví dụ: "123 Lê Lợi, Phường 1, Hà Nội"
        /// </summary>
        private string BuildFullText()
        {
            var parts = new List<string>();

            // "Số nhà Đường"
            var soNhaDuong = $"{SoNha} {Duong}".Trim();
            if (!string.IsNullOrEmpty(soNhaDuong))
                parts.Add(soNhaDuong);

            if (!string.IsNullOrEmpty(Xa))
                parts.Add(Xa);

            if (!string.IsNullOrEmpty(Tinh))
                parts.Add(Tinh);

            return string.Join(", ", parts);
        }

        /// <summary>Địa chỉ có rỗng hoàn toàn không.</summary>
        public bool IsEmpty()
            => string.IsNullOrEmpty(Tinh)
            && string.IsNullOrEmpty(Xa)
            && string.IsNullOrEmpty(Duong)
            && string.IsNullOrEmpty(SoNha);

        // ==================== EQUALITY ====================
        public bool Equals(Address? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Tinh == other.Tinh
                && Xa == other.Xa
                && Duong == other.Duong
                && SoNha == other.SoNha;
        }

        public override bool Equals(object? obj) => Equals(obj as Address);

        public override int GetHashCode()
            => HashCode.Combine(Tinh, Xa, Duong, SoNha);

        public static bool operator ==(Address? left, Address? right)
            => Equals(left, right);

        public static bool operator !=(Address? left, Address? right)
            => !Equals(left, right);

        public override string ToString() => FullText;
    }
}