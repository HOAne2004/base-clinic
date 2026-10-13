using BaseClinic.Business.Models.Department;
using BaseClinic.Business.Models.Doctor;
using BaseClinic.Business.Services.Departments.Commands;
using BaseClinic.Business.Services.Departments.Queries;
using BaseClinic.Business.Services.Doctors.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Presentation.Authorization;
using BaseClinic.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ==================== PUBLIC ENDPOINTS ====================

        /// <summary>Danh sách khoa công khai (landing page).</summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicList([FromQuery] DepartmentSearchDto search)
        {
            var result = await _mediator.Send(new GetPublicDepartmentsQuery(search));
            return Ok(new ApiResponse<object>(result));
        }

        /// <summary>Dropdown khoa đang hoạt động.</summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveList()
        {
            var result = await _mediator.Send(new GetActiveDepartmentsQuery());
            return Ok(new ApiResponse<List<DepartmentLookupDto>>(result));
        }

        /// <summary>
        /// Chi tiết khoa công khai (guest) — dùng SLUG.
        /// VD: GET /api/departments/khoa-noi-tong-hop
        /// </summary>
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicDetail(string slug)
        {
            var result = await _mediator.Send(new GetDepartmentPublicDetailQuery(slug));
            return Ok(new ApiResponse<DepartmentPublicDetailDto>(result));
        }

        // ==================== ADMIN ENDPOINTS ====================

        /// <summary>Danh sách khoa có phân trang (admin).</summary>
        [HttpGet]
        [RequirePermission(SystemPermissions.Department.View)]
        public async Task<IActionResult> GetList([FromQuery] DepartmentSearchDto search)
        {
            var result = await _mediator.Send(new GetAllDepartmentsQuery(search));
            return Ok(new ApiResponse<object>(result));
        }

        /// <summary>Chi tiết khoa cho admin (bao gồm doctors inactive).</summary>
        [HttpGet("{id:guid}/admin")]
        [RequirePermission(SystemPermissions.Department.View)]
        public async Task<IActionResult> GetAdminDetail(Guid id)
        {
            var result = await _mediator.Send(new GetDepartmentAdminDetailQuery(id));
            return Ok(new ApiResponse<DepartmentAdminDetailDto>(result));
        }

        /// <summary>Tạo khoa mới.</summary>
        [HttpPost]
        [RequirePermission(SystemPermissions.Department.Create)]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new ApiResponse<Guid>(id, "Tạo khoa thành công."));
        }

        /// <summary>Cập nhật thông tin khoa.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(SystemPermissions.Department.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentCommand command)
        {
            var finalCommand = command with { Id = id };
            await _mediator.Send(finalCommand);
            return Ok(new ApiResponse<bool>(true, "Cập nhật khoa thành công."));
        }

        /// <summary>Tạm ngưng khoa (cascade deactivate doctors).</summary>
        [HttpPost("{id:guid}/deactivate")]
        [RequirePermission(SystemPermissions.Department.Deactivate)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateDepartmentCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã tạm ngưng khoa."));
        }

        /// <summary>Kích hoạt lại khoa.</summary>
        [HttpPost("{id:guid}/activate")]
        [RequirePermission(SystemPermissions.Department.Activate)]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivateDepartmentCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã kích hoạt lại khoa."));
        }

        /// <summary>Xóa mềm khoa (phải hết doctors).</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(SystemPermissions.Department.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteDepartmentCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã xóa khoa."));
        }

        /// <summary>Khôi phục khoa đã xóa mềm.</summary>
        [HttpPost("{id:guid}/restore")]
        [RequirePermission(SystemPermissions.Department.Restore)]
        public async Task<IActionResult> Restore(Guid id)
        {
            await _mediator.Send(new RestoreDepartmentCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã khôi phục khoa."));
        }

        /// <summary>
        /// Danh sách bác sĩ đang hoạt động của một khoa — dùng cho public.
        /// URL: GET /api/departments/{slug}/doctors
        /// VD: GET /api/departments/khoa-nhi/doctors
        /// </summary>
        [HttpGet("{slug}/doctors")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorsByDepartment(string slug)
        {
            var result = await _mediator.Send(new GetDoctorsByDepartmentQuery(slug));
            return Ok(new ApiResponse<List<DoctorPublicListDto>>(result));
        }
    }
}