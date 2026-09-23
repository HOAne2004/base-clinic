namespace BaseClinic.Presentation.Middlewares
{
    /// <summary>
    /// [DOMAIN EXCEPTION: LỖI NGHIỆP VỤ LÕI]
    /// Chức năng: Exception độc quyền dành riêng cho tầng Domain.
    /// Đặc điểm: 
    /// - CHỈ ĐƯỢC ném ra (throw) bên trong các class Entity/Aggregate Root/Value Object khi vi phạm các quy tắc nghiệp vụ cốt lõi (Invariants).
    /// - Ví dụ: Ném ra khi cố gắng "Hủy" một Đơn hàng đã ở trạng thái "Giao thành công".
    /// - GlobalExceptionHandler ở tầng API sẽ bắt lỗi này và tự động chuyển thành HTTP 400 Bad Request cho Frontend.
    /// </summary>
    public class DomainException : Exception
    {
        // Có thể mở rộng thêm mã lỗi (ErrorCode) nếu dự án cần đa ngôn ngữ (i18n) sau này
        // public string ErrorCode { get; }

        public DomainException(string message)
            : base(message)
        {
        }

        // Constructor dùng khi có ErrorCode
        // public DomainException(string errorCode, string message) : base(message)
        // {
        //     ErrorCode = errorCode;
        // }
    }
}
