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

        // ==========================================
        // 1. QUẢN LÝ THỰC THỂ ROLE (CRUD)
        // ==========================================

        [HttpGet]
        [RequirePermission(SystemPermissions.Role.View)]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            return Ok(result);
        }

        [HttpPost]
        [RequirePermission(SystemPermissions.Role.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var roleId = await _mediator.Send(command);
            return Ok(new { Message = "Tạo mới chức danh thành công.", Data = roleId });
        }

        [HttpPut("{id}")]
        [RequirePermission(SystemPermissions.Role.Update)]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
        {
            // Sử dụng "with" để copy record và gán đè Id từ URL vào Command
            var finalCommand = command with { Id = id };
            await _mediator.Send(finalCommand);
            return Ok(new { Message = "Cập nhật chức danh thành công." });
        }

        [HttpDelete("{id}")]
        [RequirePermission(SystemPermissions.Role.Delete)]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            await _mediator.Send(new DeleteRoleCommand(id));
            return Ok(new { Message = "Đã xóa chức danh thành công." });
        }

        // ==========================================
        // 2. QUẢN LÝ PHÂN QUYỀN CỦA ROLE
        // ==========================================

        // Lấy danh sách toàn bộ quyền theo nhóm (Vẽ Checkbox)
        [HttpGet("permissions")]
        [RequirePermission(SystemPermissions.Permission.View)]
        public async Task<IActionResult> GetAllPermission()
        {
            var query = new GetAllPermissionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Lấy các ID quyền hiện tại của 1 Role (Tích sẵn Checkbox)
        [HttpGet("{id}/permissions")]
        [RequirePermission(SystemPermissions.RolePermission.View)]
        public async Task<IActionResult> GetPermissionsByRole(Guid id)
        {
            // Sửa lỗi: Chuyển cú pháp khởi tạo ngoặc nhọn sang ngoặc tròn do đây là record
            var query = new GetRolePermissionsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // PUT lưu danh sách quyền mới vào Role
        [HttpPut("{id}/permissions")]
        [RequirePermission(SystemPermissions.RolePermission.Assign)]
        public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] List<Guid> permissionIds)
        {
            // Sửa lỗi: Chuyển cú pháp khởi tạo ngoặc nhọn sang ngoặc tròn do đây là record
            var command = new AssignRolePermissionsCommand(id, permissionIds);

            await _mediator.Send(command);
            return Ok(new { Message = "Cập nhật phân quyền thành công." });
        }
    }
}