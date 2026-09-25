using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseClinic.Presentation.Authorization
{
    /// <summary>
    /// Attribute dùng để đánh dấu yêu cầu quyền cụ thể trên Controller hoặc Action
    /// </summary>
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string permission) : base(typeof(RequirePermissionFilter))
        {
            // Truyền mã quyền (ví dụ: "Appointment.Book") vào Filter
            Arguments = new object[] { permission };
        }
    }

    /// <summary>
    /// Filter thực thi logic kiểm tra JWT Token
    /// </summary>
    public class RequirePermissionFilter : IAuthorizationFilter
    {
        private readonly string _permission;

        public RequirePermissionFilter(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 1. Kiểm tra xem request có Token hợp lệ không
            var user = context.HttpContext.User;
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                // Trả về JSON thông báo 401 thay vì UnauthorizedResult rỗng
                context.Result = new ObjectResult(new
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Chưa xác thực.",
                    Detail = "Vui lòng đăng nhập để sử dụng tính năng này."
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // 2. Quét trong danh sách Claims của Token xem có chứa mã quyền yêu cầu không.
            bool hasPermission = user.Claims.Any(c =>
                c.Type == "Permission" && c.Value == _permission);

            // 3. Nếu không có quyền, chặn Request và trả về mã 403 Forbidden dạng JSON
            if (!hasPermission)
            {
                context.Result = new ObjectResult(new
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Không có quyền truy cập.",
                    Detail = $"Bạn cần quyền '{_permission}' để thực hiện thao tác này."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}