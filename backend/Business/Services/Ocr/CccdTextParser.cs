using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BaseClinic.Business.Models.Ocr;

namespace BaseClinic.Business.Services.Ocr
{
    /// <summary>
    /// Parser chuyển văn bản thô từ Tesseract thành structured data.
    /// 
    /// CCCD Việt Nam có layout:
    ///     CĂN CƯỚC CÔNG DÂN
    ///     Số: 001199001234
    ///     Họ và tên: NGUYỄN VĂN A
    ///     Ngày sinh: 15/01/1990     Giới tính: Nam
    ///     Quốc tịch: Việt Nam
    ///     Quê quán: ...
    ///     Nơi thường trú: ...
    ///     Có giá trị đến: 15/01/2030
    /// </summary>
    public static class CccdTextParser
    {
        public static CccdOcrResultDto Parse(string rawText, float confidence, float minConfidence)
        {
            // Normalize: bỏ khoảng trắng thừa, chuẩn hóa dấu xuống dòng
            var text = NormalizeText(rawText);

            return new CccdOcrResultDto(
                IdentityNumber: ExtractIdNumber(text),
                FullName: ExtractFullName(text),
                DateOfBirth: ExtractDateOfBirth(text),
                Gender: ExtractGender(text),
                Nationality: ExtractNationality(text),
                PlaceOfOrigin: ExtractPlaceOfOrigin(text),
                PlaceOfResidence: ExtractPlaceOfResidence(text),
                ExpiryDate: ExtractExpiryDate(text),
                RawText: rawText,
                Confidence: confidence,
                RequiresManualReview: confidence < minConfidence
            );
        }

        // ==================== NORMALIZE ====================
        private static string NormalizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Chuẩn hóa Unicode (NFC) — gộp dấu tiếng Việt
            var normalized = input.Normalize(NormalizationForm.FormC);

            // Thay thế các ký tự có thể bị OCR sai
            normalized = normalized
                .Replace('–', '-')
                .Replace('—', '-')
                .Replace(':', ':');

            // Bỏ khoảng trắng thừa nhưng giữ xuống dòng
            var lines = normalized.Split('\n')
                .Select(l => Regex.Replace(l, @"\s+", " ").Trim())
                .Where(l => !string.IsNullOrEmpty(l));

            return string.Join("\n", lines);
        }

        // ==================== EXTRACT METHODS ====================

        /// <summary>
        /// Số CCCD: 12 chữ số liên tiếp.
        /// </summary>
        private static string? ExtractIdNumber(string text)
        {
            // Ưu tiên: "Số: 001199001234" hoặc "Số / No.: 001199001234"
            var matchWithLabel = Regex.Match(text,
                @"(?:Số|So|No)\s*[:\.]?\s*(\d{12})",
                RegexOptions.IgnoreCase);

            if (matchWithLabel.Success)
                return matchWithLabel.Groups[1].Value;

            // Fallback: tìm chuỗi 12 số bất kỳ
            var match = Regex.Match(text, @"\b(\d{12})\b");
            return match.Success ? match.Groups[1].Value : null;
        }

        /// <summary>
        /// Họ và tên: dòng sau "Họ và tên" hoặc "Full name".
        /// Tên thường viết HOA.
        /// </summary>
        private static string? ExtractFullName(string text)
        {
            // Trường hợp 1: cùng dòng "Họ và tên: NGUYỄN VĂN A"
            var sameLineMatch = Regex.Match(text,
                @"(?:Họ và tên|Ho va ten|Full name)\s*[:\.]?\s*([^\n]+)",
                RegexOptions.IgnoreCase);

            if (sameLineMatch.Success)
            {
                var name = sameLineMatch.Groups[1].Value.Trim();
                // Bỏ text không phải tên (VD: "Ngày sinh")
                if (!string.IsNullOrWhiteSpace(name) && name.Length <= 100)
                    return CleanName(name);
            }

            // Trường hợp 2: tên ở dòng riêng sau "Họ và tên"
            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (Regex.IsMatch(lines[i], @"Họ và tên|Full name", RegexOptions.IgnoreCase))
                {
                    var nextLine = lines[i + 1].Trim();
                    if (!string.IsNullOrWhiteSpace(nextLine) && nextLine.Length <= 100)
                        return CleanName(nextLine);
                }
            }

            return null;
        }

        private static string CleanName(string name)
        {
            // Bỏ các ký tự không phải chữ cái, khoảng trắng, dấu tiếng Việt
            name = Regex.Replace(name, @"[^\p{L}\s]", " ");
            name = Regex.Replace(name, @"\s+", " ").Trim();

            // Title case nếu cần (CCCD thường viết HOA — giữ nguyên)
            return name;
        }

        /// <summary>
        /// Ngày sinh: dd/MM/yyyy.
        /// </summary>
        private static DateTime? ExtractDateOfBirth(string text)
        {
            var match = Regex.Match(text,
                @"(?:Ngày sinh|Date of birth)\s*[:\.]?\s*(\d{1,2})[/\-\.](\d{1,2})[/\-\.](\d{4})",
                RegexOptions.IgnoreCase);

            if (match.Success)
                return TryParseDate(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);

            return null;
        }

        /// <summary>
        /// Giới tính: Nam/Nữ.
        /// </summary>
        private static bool? ExtractGender(string text)
        {
            var match = Regex.Match(text,
                @"(?:Giới tính|Sex)\s*[:\.]?\s*(\S+)",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var value = match.Groups[1].Value.Trim();
                if (value.Equals("Nam", StringComparison.OrdinalIgnoreCase)) return true;
                if (value.Equals("Nữ", StringComparison.OrdinalIgnoreCase)) return false;
                if (value.Equals("Nu", StringComparison.OrdinalIgnoreCase)) return false;
            }

            // Fallback: tìm "Nam" hoặc "Nữ" trên dòng chứa "Giới tính"
            if (Regex.IsMatch(text, @"\bNam\b")) return true;
            if (Regex.IsMatch(text, @"\bNữ\b")) return false;

            return null;
        }

        private static string? ExtractNationality(string text)
        {
            var match = Regex.Match(text,
                @"(?:Quốc tịch|Nationality)\s*[:\.]?\s*([^\n]+)",
                RegexOptions.IgnoreCase);

            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        private static string? ExtractPlaceOfOrigin(string text)
        {
            var match = Regex.Match(text,
                @"(?:Quê quán|Place of origin)\s*[:\.]?\s*([^\n]+)",
                RegexOptions.IgnoreCase);

            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        private static string? ExtractPlaceOfResidence(string text)
        {
            // Nơi thường trú có thể chiếm nhiều dòng → lấy đến dòng tiếp theo chứa keyword
            var match = Regex.Match(text,
                @"(?:Nơi thường trú|Place of residence)\s*[:\.]?\s*([\s\S]+?)(?=\n(?:Có giá trị|Date of expiry|$))",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var address = match.Groups[1].Value.Trim();
                address = Regex.Replace(address, @"\s+", " ");
                return address;
            }

            return null;
        }

        private static DateTime? ExtractExpiryDate(string text)
        {
            var match = Regex.Match(text,
                @"(?:Có giá trị đến|Date of expiry|Có giá trị|Expiry)\s*[:\.]?\s*(\d{1,2})[/\-\.](\d{1,2})[/\-\.](\d{4})",
                RegexOptions.IgnoreCase);

            if (match.Success)
                return TryParseDate(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);

            return null;
        }

        private static DateTime? TryParseDate(string day, string month, string year)
        {
            var full = $"{day}/{month}/{year}";
            if (DateTime.TryParseExact(full, "d/M/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result))
            {
                return result;
            }
            return null;
        }
    }
}