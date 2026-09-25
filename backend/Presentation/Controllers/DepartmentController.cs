using BaseClinic.Business.Services.Departments.Commands;
using BaseClinic.Business.Services.Departments.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [RequirePermission(SystemPermissions.Department.Create)]
        public async Task<IActionResult> Create([FromBody] CreateDeparementCommand command)
        {
            var departmentId = await _mediator.Send(command);

            return Ok(new
            {
                Message = "Tạo mới thành công.",
                Data = departmentId
            });
        }

        [HttpPut("{id}")]
        [RequirePermission(SystemPermissions.Department.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentCommand command)
        {
            var finalCommand = command with { Id = id };
            await _mediator.Send(command);
            return Ok(new { Message = "Cập nhật thông tin thành công." });
        }

        [HttpDelete("{id}")]
        [RequirePermission(SystemPermissions.Department.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeactivateDepartmentCommand( id );
            await _mediator.Send(command);
            return Ok(new { Message = "Đã vô hiệu hóa thành công." });
        }

        [HttpGet("lookup")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLookupList()
        {
            var query = new GetActiveDepartmentsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [RequirePermission(SystemPermissions.Department.View)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllDepartmentsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
