using BaseClinic.Business.Services.Roles.Commands;
using BaseClinic.Business.Services.Roles.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Presentation.Authorization;
using BaseClinic.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        /// <summary>
        /// Đọc AccountId của Admin đang đăng nhập từ JWT.
        /// Dùng làm GrantedBy khi gán permission.
        /// </summary>
        private Guid GetCurrentAccountId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(claim, out var accountId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");
            return accountId;
        }

        // ==========================================
        // 1. QUẢN LÝ ROLE (CRUD)
        // ==========================================

        [HttpGet]
        [RequirePermission(SystemPermissions.Role.View)]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            return Ok(new ApiResponse<object>(result));
        }

        [HttpPost]
        [RequirePermission(SystemPermissions.Role.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var roleId = await _mediator.Send(command);
            return Ok(new ApiResponse<Guid>(roleId, "Tạo mới chức danh thành công."));
        }

        [HttpPut("{id:guid}")]
        [RequirePermission(SystemPermissions.Role.Update)]
        public async Task<IActionResult> UpdateRole(
            Guid id, [FromBody] UpdateRoleCommand command)
        {
            var finalCommand = command with { Id = id };
            await _mediator.Send(finalCommand);
            return Ok(new ApiResponse<bool>(true, "Cập nhật chức danh thành công."));
        }

        [HttpDelete("{id:guid}")]
        [RequirePermission(SystemPermissions.Role.Delete)]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            await _mediator.Send(new DeleteRoleCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã xóa chức danh thành công."));
        }

        // ==========================================
        // 2. QUẢN LÝ PHÂN QUYỀN CỦA ROLE
        // ==========================================

        [HttpGet("permissions")]
        [RequirePermission(SystemPermissions.Permission.View)]
        public async Task<IActionResult> GetAllPermission()
        {
            var result = await _mediator.Send(new GetAllPermissionsQuery());
            return Ok(new ApiResponse<object>(result));
        }

        [HttpGet("{id:guid}/permissions")]
        [RequirePermission(SystemPermissions.RolePermission.View)]
        public async Task<IActionResult> GetPermissionsByRole(Guid id)
        {
            var result = await _mediator.Send(new GetRolePermissionsQuery(id));
            return Ok(new ApiResponse<object>(result));
        }

        /// <summary>
        /// Gán danh sách permission cho role.
        /// GrantedByAccountId được lấy từ JWT → audit chính xác ai gán.
        /// </summary>
        [HttpPut("{id:guid}/permissions")]
        [RequirePermission(SystemPermissions.RolePermission.Assign)]
        public async Task<IActionResult> AssignPermissions(
            Guid id, [FromBody] List<Guid> permissionIds)
        {
            var currentAccountId = GetCurrentAccountId();

            var command = new AssignRolePermissionsCommand(
                RoleId: id,
                PermissionIds: permissionIds,
                GrantedByAccountId: currentAccountId);

            await _mediator.Send(command);
            return Ok(new ApiResponse<bool>(true, "Cập nhật phân quyền thành công."));
        }
    }
}