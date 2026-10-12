using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Ocr;
using BaseClinic.Business.Services.Ocr;
using BaseClinic.Business.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tesseract;

namespace BaseClinic.DataAccess.Services
{
    /// <summary>
    /// OCR Service sử dụng Tesseract (offline, mã nguồn mở).
    /// 
    /// YÊU CẦU:
    /// - NuGet package: Tesseract 5.2.0.
    /// - File "vie.traineddata" trong thư mục OcrOptions.TessDataPath.
    /// </summary>
    public class TesseractOcrService : IOcrService
    {
        private readonly OcrOptions _options;
        private readonly ILogger<TesseractOcrService> _logger;

        public TesseractOcrService(
            IOptions<OcrOptions> options,
            ILogger<TesseractOcrService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task<CccdOcrResultDto> ExtractCccdAsync(
            Stream imageStream,
            CancellationToken cancellationToken = default)
        {
            // ⚡ Tesseract.NET không hỗ trợ async thực sự — chạy sync trong Task.Run
            return Task.Run(() => ExtractCccdInternal(imageStream), cancellationToken);
        }

        private CccdOcrResultDto ExtractCccdInternal(Stream imageStream)
        {
            // ==================== BƯỚC 1: Đọc ảnh thành bytes ====================
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                imageStream.CopyTo(ms);
                imageBytes = ms.ToArray();
            }

            if (imageBytes.Length == 0)
                throw new InvalidOperationException("Ảnh rỗng.");

            // ==================== BƯỚC 2: Khởi tạo Tesseract ====================
            _logger.LogInformation("Bắt đầu OCR CCCD, kích thước ảnh: {Size} bytes",
                imageBytes.Length);

            using var engine = new TesseractEngine(
                _options.TessDataPath,
                _options.Language,
                EngineMode.Default);

            // Set PSM = Auto (tự động phát hiện layout)
            engine.DefaultPageSegMode = PageSegMode.Auto;

            // Cấu hình engine — giữ khoảng trắng giữa các từ
            engine.SetVariable("preserve_interword_spaces", "1");

            // ==================== BƯỚC 3: Load ảnh vào Pix ====================
            using var pix = Pix.LoadFromMemory(imageBytes);

            // ==================== BƯỚC 4: Chạy OCR ====================
            using var page = engine.Process(pix);

            var rawText = page.GetText() ?? string.Empty;
            var confidence = page.GetMeanConfidence();

            _logger.LogInformation(
                "OCR hoàn tất. Độ tin cậy: {Confidence:P0}, Text length: {Length}",
                confidence, rawText.Length);

            // ==================== BƯỚC 5: Parse ====================
            var result = CccdTextParser.Parse(
                rawText,
                confidence,
                _options.MinConfidence);

            return result;
        }
    }
}