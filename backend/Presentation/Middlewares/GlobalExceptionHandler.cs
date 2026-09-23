using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace BaseClinic.Presentation.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            if (exception is ValidationException fluentException)
            {
                problemDetails.Title = "Dữ liệu đầu vào không hợp lệ.";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Extensions.Add("errors", fluentException.Errors.Select(e => e.ErrorMessage));
            }
            else if (exception is DomainException domainException)
            {
                problemDetails.Title = "Lỗi quy tắc nghiệp vụ.";
                // Với lỗi nghiệp vụ, mã 400 (Bad Request) hoặc 422 (Unprocessable Entity) đều chuẩn.
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = domainException.Message;
            }
            else if (exception is DbUpdateConcurrencyException concurrencyException)
            {
                // Log riêng chi tiết Entity nào gây xung đột - đây là thông tin EF Core cung cấp sẵn
                // nhưng bị bỏ qua nếu chỉ đọc exception.Message chung chung.
                foreach (var entry in concurrencyException.Entries)
                {
                    _logger.LogError(
                        "DbUpdateConcurrencyException chi tiết - Entity: {EntityType}, State: {State}, KeyValues: {KeyValues}",
                        entry.Entity.GetType().Name,
                        entry.State,
                        string.Join(", ", entry.Properties
                            .Where(p => p.Metadata.IsPrimaryKey())
                            .Select(p => $"{p.Metadata.Name}={p.CurrentValue}")));
                }

                problemDetails.Title = "Dữ liệu đã bị thay đổi.";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Detail = "Dữ liệu này vừa được cập nhật hoặc không còn tồn tại. Vui lòng tải lại trang và thử lại.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                problemDetails.Title = "Lỗi xác thực.";
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Detail = exception.Message;
            }
            else
            {
                // Mọi lỗi hệ thống (chia 0, null reference, lỗi EF Core) sẽ lọt vào đây
                problemDetails.Title = "Lỗi hệ thống nội bộ.";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                // Bật dòng này ở môi trường Dev, nhưng lên Production nên ghi log ẩn đi
                problemDetails.Detail = exception.Message;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            try
            {
                // Dùng CancellationToken.None thay vì token đã truyền vào: nếu client đã ngắt
                // kết nối (timeout phía FE), việc "hủy" thao tác ghi không còn ý nghĩa gì -
                // chỉ cần thử ghi, nếu thất bại vì kết nối đã đóng thì bỏ qua lặng lẽ bên dưới,
                // tránh tạo ra MỘT exception thứ hai đè lên exception gốc đã log ở trên.
                await httpContext.Response.WriteAsJsonAsync(problemDetails, CancellationToken.None);
            }
            catch (Exception)
            {
                // Client đã ngắt kết nối trước khi kịp nhận response lỗi - không còn gì để làm,
                // nhưng exception gốc đã được log ở trên nên không mất thông tin debug.
            }
            return true;
        }
    }
}
