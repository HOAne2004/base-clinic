namespace BaseClinic.Business.Models.Ocr
{
    /// <summary>
    /// Kết quả OCR từ ảnh CCCD.
    /// Tất cả field có thể null nếu không nhận dạng được.
    /// </summary>
    public record CccdOcrResultDto(
        /// <summary>Số CCCD 12 số.</summary>
        string? IdentityNumber,

        /// <summary>Họ và tên.</summary>
        string? FullName,

        /// <summary>Ngày sinh.</summary>
        DateTime? DateOfBirth,

        /// <summary>Giới tính (true = Nam, false = Nữ).</summary>
        bool? Gender,

        /// <summary>Quốc tịch.</summary>
        string? Nationality,

        /// <summary>Quê quán.</summary>
        string? PlaceOfOrigin,

        /// <summary>Nơi thường trú (địa chỉ).</summary>
        string? PlaceOfResidence,

        /// <summary>Ngày hết hạn.</summary>
        DateTime? ExpiryDate,

        /// <summary>Văn bản thô từ Tesseract — để debug/audit.</summary>
        string RawText,

        /// <summary>Độ tin cậy tổng thể (0-1).</summary>
        float Confidence,

        /// <summary>True nếu confidence &lt; ngưỡng — cần user review thủ công.</summary>
        bool RequiresManualReview
    );
}