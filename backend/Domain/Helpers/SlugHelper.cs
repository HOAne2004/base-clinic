using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseClinic.Domain.Helpers
{
    /// <summary>
    /// Helper chuyển string thành slug (URL-friendly).
    /// THUẦN LOGIC — không phụ thuộc DB, dùng được ở cả Entity, Seeder, Service.
    /// 
    /// QUY TẮC:
    /// 1. Chuẩn hóa Unicode FormD → tách dấu.
    /// 2. Bỏ dấu tiếng Việt.
    /// 3. Lowercase.
    /// 4. Đổi 'đ' → 'd'.
    /// 5. Bỏ ký tự đặc biệt (chỉ giữ a-z, 0-9, space, hyphen).
    /// 6. Space → hyphen.
    /// 7. Gộp nhiều hyphen liên tiếp.
    /// 8. Trim hyphen đầu/cuối.
    /// </summary>
    public static class SlugHelper
    {
        /// <summary>
        /// Sinh slug từ input. Trả về string rỗng nếu input không hợp lệ.
        /// </summary>
        public static string Generate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1. Chuẩn hóa FormD để tách dấu
            var normalized = input.Normalize(NormalizationForm.FormD);

            // 2. Bỏ các combining mark (dấu)
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            // 3. Về FormC
            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            // 4. Lowercase
            noDiacritics = noDiacritics.ToLowerInvariant();

            // 5. Đặc biệt: 'đ' → 'd' (không bị FormD tách)
            noDiacritics = noDiacritics.Replace('đ', 'd');

            // 6. Bỏ ký tự không phải a-z, 0-9, space, hyphen
            noDiacritics = Regex.Replace(noDiacritics, @"[^a-z0-9\s-]", "");

            // 7. Space → hyphen
            noDiacritics = Regex.Replace(noDiacritics, @"\s+", "-");

            // 8. Gộp hyphen liên tiếp
            noDiacritics = Regex.Replace(noDiacritics, @"-+", "-");

            // 9. Trim hyphen
            return noDiacritics.Trim('-');
        }
    }
}