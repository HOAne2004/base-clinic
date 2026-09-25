using BaseClinic.Presentation.Authorization;
using BaseClinic.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseClinic.Business.Services.Doctors.Commands;

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
        public async Task<IActionResult> Create([FromBody] CreateDoctorCommand command, CancellationToken cancellation)
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
    }
}
