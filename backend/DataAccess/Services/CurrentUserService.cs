using BaseClinic.Business.Interfaces;
using System.Security.Claims;

namespace BaseClinic.DataAccess.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        /// <summary>
        /// Service lấy thông tin người dùng hiện tại từ HttpContext.
        /// Được inject vào ClinicDbContext để tự động điền audit fields.
        /// Trả về null nếu không có HttpContext (ví dụ: background job, seed data).
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Id tài khoản đang đăng nhập, đọc từ JWT claim NameIdentifier.
        /// Trả về null nếu hệ thống tự chạy hoặc user chưa đăng nhập.
        /// </summary>
        public Guid? AccountId
        {
            get
            {
                var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }

                return null; // Trả về null nếu hệ thống tự chạy hoặc user chưa đăng nhập
            }
        }

        /// <summary>
        /// Tên hiển thị (họ tên) của tài khoản đang đăng nhập, đọc từ JWT claim Name.
        /// Dùng để lưu vào trường CreatedBy/UpdatedBy (dạng string, dễ đọc log).
        /// Trả về null nếu không có HttpContext.
        /// </summary>
        public string? UserName
        {
            get
            {
                return _contextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.Name)?.Value;
            }
        }

        /// <summary>
        /// Kiểm tra user hiện tại có role cụ thể hay không.
        /// Dùng cho authorization logic trong Service.
        /// </summary>
        public bool IsInRole(string roleName)
        {
            return _contextAccessor.HttpContext?.User?.HasClaim(ClaimTypes.Role, roleName) ?? false;
        }
    }
}
