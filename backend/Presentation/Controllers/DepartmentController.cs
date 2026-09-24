using BaseClinic.Business.Services.Departments.Commands;
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

            return CreatedAtAction(
                actionName: nameof(Create),
                routeValues: new { id = departmentId },
                value: new { Message = "Thêm mới khoa thành công.", Data = departmentId });
        }
    }
}
