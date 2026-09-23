using BaseClinic.Business.Services.Roles.Commands;
using BaseClinic.Business.Services.Roles.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Lấy danh sách toàn bộ quyền theo nhóm (Vẽ Checkbox)
        [HttpGet("permissions")]
        [RequirePermission(SystemPermissions.Role.View)]
        public async Task<IActionResult> GetAllPermission()
        {
            var query = new GetAllPermissionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Lấy các ID quyền hiện tại của 1 Role (Tích sẵn Checkbox)
        [HttpGet("{id}/permissions")]
        [RequirePermission(SystemPermissions.Role.View)]
        public async Task<IActionResult> GetPermissionsByRole(Guid id)
        {
            var query = new GetRolePermissionsQuery { RoleId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // PUT lưu danh sách quyền mới vào Role
        [HttpPut("{id}/permissions")]
        [RequirePermission(SystemPermissions.Role.Update)]
        public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] List<Guid> permissionIds)
        {
            var command = new AssignRolePermissionsCommand
            {
                RoleId = id,
                PermissionIds = permissionIds
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Cập nhật phân quyền thành công." });
        }
    }
}
