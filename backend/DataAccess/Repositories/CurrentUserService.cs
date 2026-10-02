using BaseClinic.Business.Interfaces;
using System.Security.Claims;

namespace BaseClinic.DataAccess.Repositories
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

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

        public bool IsInRole(string roleName)
        {
            return _contextAccessor.HttpContext?.User?.HasClaim(ClaimTypes.Role, roleName) ?? false;
        }
    }
}
