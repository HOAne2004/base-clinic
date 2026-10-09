using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using BaseClinic.Business.Interfaces.Repositories;

namespace BaseClinic.Presentation.Authorization
{
    /// <summary>
    /// Attribute yêu cầu permission cụ thể trên Controller/Action.
    /// 
    /// CÁCH HOẠT ĐỘNG:
    /// 1. Đọc roles từ JWT claims.
    /// 2. Query permissions của các roles đó (cache 10 phút).
    /// 3. Union permissions lại.
    /// 4. Check xem có permission yêu cầu không.
    /// </summary>
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string permission)
            : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class RequirePermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        private readonly IMemoryCache _cache;
        private readonly IRoleRepository _roleRepository;

        public RequirePermissionFilter(
            string permission,
            IMemoryCache cache,
            IRoleRepository roleRepository)
        {
            _permission = permission;
            _cache = cache;
            _roleRepository = roleRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // 1. Check authenticated
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
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

            // 2. Lấy roles từ JWT
            var roles = user.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (roles.Count == 0)
            {
                context.Result = CreateForbiddenResult();
                return;
            }

            // 3. Union permissions của tất cả roles
            var permissions = new HashSet<string>();
            foreach (var roleCode in roles)
            {
                var cacheKey = $"role_permissions_{roleCode}";

                if (!_cache.TryGetValue(cacheKey, out HashSet<string>? rolePerms))
                {
                    rolePerms = await _roleRepository.GetPermissionCodesByRoleCodeAsync(roleCode);
                    _cache.Set(cacheKey, rolePerms, TimeSpan.FromMinutes(10));
                }

                if (rolePerms != null)
                    permissions.UnionWith(rolePerms);
            }

            // 4. Check permission
            if (!permissions.Contains(_permission))
            {
                context.Result = CreateForbiddenResult();
            }
        }

        private ObjectResult CreateForbiddenResult()
        {
            return new ObjectResult(new
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