using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    public class BaseClinicController : ControllerBase
    {
        protected Guid? UserId
            => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

        protected string FullName
            => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        protected List<string>? Roles
            => User.FindFirst(ClaimTypes.Role)?.Value?.Split(',').ToList();

        protected bool HasRole(string role)
            => Roles != null && Roles.Contains(role);

        protected IEnumerable<string> ModelStateError
            => ModelState.Values.SelectMany(v => v.Errors.Select(x => x.ErrorMessage));
    }
}
