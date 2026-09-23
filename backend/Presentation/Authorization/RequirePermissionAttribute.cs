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
            // 1. Kiểm tra xem request có Token hợp lệ không (đã qua lớp [Authorize] mặc định chưa)
            var user = context.HttpContext.User;
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult(); // Trả về 401
                return;
            }

            // 2. Quét trong danh sách Claims của Token xem có chứa mã quyền yêu cầu không.
            // Chú ý: Chuỗi "Permission" ở đây phải khớp tuyệt đối với Claim định nghĩa trong JwtProvider_2.cs
            bool hasPermission = user.Claims.Any(c =>
                c.Type == "Permission" && c.Value == _permission);

            // 3. Nếu không có quyền, chặn Request và trả về mã 403 Forbidden
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }

    }
}
