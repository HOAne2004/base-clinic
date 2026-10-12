using BaseClinic.Business.Models.Ocr;

namespace BaseClinic.Business.Interfaces.Services
{
    /// <summary>
    /// Trừu tượng hóa OCR — cho phép thay đổi provider sau này.
    /// 
    /// Implementations:
    /// - TesseractOcrService (mã nguồn mở, offline).
    /// - FptAiOcrService (thương mại, cloud) — có thể thêm sau.
    /// </summary>
    public interface IOcrService
    {
        /// <summary>
        /// Đọc thông tin CCCD từ ảnh.
        /// </summary>
        /// <param name="imageStream">Stream ảnh (JPEG/PNG).</param>
        /// <returns>Kết quả OCR — các field có thể null nếu không nhận dạng được.</returns>
        Task<CccdOcrResultDto> ExtractCccdAsync(
            Stream imageStream,
            CancellationToken cancellationToken = default);
    }
}