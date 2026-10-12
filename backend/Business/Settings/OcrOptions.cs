namespace BaseClinic.Business.Settings
{
    /// <summary>
    /// Cấu hình OCR.
    /// </summary>
    public class OcrOptions
    {
        public const string SectionName = "OcrSettings";

        /// <summary>
        /// Đường dẫn tới thư mục chứa file .traineddata.
        /// Mặc định: "tessdata" (cùng cấp với file .exe).
        /// </summary>
        public string TessDataPath { get; init; } = "tessdata";

        /// <summary>
        /// Ngôn ngữ nhận dạng — "vie" cho tiếng Việt.
        /// Có thể kết hợp: "vie+eng" cho cả 2.
        /// </summary>
        public string Language { get; init; } = "vie";

        /// <summary>
        /// Ngưỡng confidence tối thiểu (0-1).
        /// Nếu dưới ngưỡng → đánh dấu kết quả là "cần review".
        /// </summary>
        public float MinConfidence { get; init; } = 0.5f;

        /// <summary>
        /// Giới hạn kích thước file upload (MB).
        /// </summary>
        public int MaxFileSizeMb { get; init; } = 5;
    }
}