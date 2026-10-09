using BaseClinic.Business.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace BaseClinic.Presentation.Middlewares
{
    public class SecurityStampValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityStampValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IAccountRepository accountRepo,
            IMemoryCache cache)
        {
            // Nếu chưa authenticated → skip
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            // Đọc claim
            var accountIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var securityStampClaim = context.User.FindFirst("SecurityStamp")?.Value;

            if (!Guid.TryParse(accountIdClaim, out var accountId) ||
                !Guid.TryParse(securityStampClaim, out var tokenStamp))
            {
                context.Response.StatusCode = 401;
                return;
            }

            // Cache key
            var cacheKey = $"security_stamp_{accountId}";

            if (!cache.TryGetValue<Guid>(cacheKey, out var currentStamp))
            {
                var account = await accountRepo.GetByIdAsync(accountId);
                if (account == null)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                currentStamp = account.SecurityStamp;
                cache.Set(cacheKey, currentStamp, TimeSpan.FromMinutes(5));
            }

            // So sánh
            if (currentStamp != tokenStamp)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Token đã hết hiệu lực. Vui lòng đăng nhập lại."
                });
                return;
            }

            await _next(context);
        }
    }
}
