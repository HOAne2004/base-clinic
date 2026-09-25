using BaseClinic.Business.Services.Doctors.Commands;
using BaseClinic.Business.Services.Doctors.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DoctorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [RequirePermission(SystemPermissions.Doctor.Create)]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorCommand command, CancellationToken cancellation)
        {
            // Gửi Command vào Mediator pipeline để xử lý nghiệp vụ
            var doctorId = await _mediator.Send(command);

            // Trả về mã 200 OK cùng ID của bác sĩ vừa tạo
            return Ok(new
            {
                Message = "Thêm mới bác sĩ thành công.",
                Data = doctorId
            });
        }

        [HttpPut("{id}")]
        [RequirePermission(SystemPermissions.Doctor.Update)]
        public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] UpdateDoctorCommand command)
        {
            // Gán đè ID từ URL vào Command để tránh việc FE truyền sai lệch ID trong Body
            var finalCommand = command with { Id = id };

            await _mediator.Send(finalCommand);

            return Ok(new { Message = "Cập nhật hồ sơ bác sĩ thành công." });
        }

        [HttpDelete("{id}")]
        [RequirePermission(SystemPermissions.Doctor.Delete)]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            await _mediator.Send(new DeleteDoctorCommand(id));

            return Ok(new { Message = "Đã xóa bác sĩ và vô hiệu hóa tài khoản liên kết." });
        }

        [HttpGet]
        [RequirePermission(SystemPermissions.Doctor.View)]
        public async Task<IActionResult> GetDoctorsForAdmin()
        {
            var result = await _mediator.Send(new GetAllDoctorsQuery());
            return Ok(result);
        }

        [HttpGet("{id}/admin-detail")]
        [RequirePermission(SystemPermissions.Doctor.Manage)]
        public async Task<IActionResult> GetDoctorDetailForAdmin(Guid id)
        {
            var result = await _mediator.Send(new GetDoctorDetailForAdminQuery(id));
            return Ok(result);
        }
    }
}
