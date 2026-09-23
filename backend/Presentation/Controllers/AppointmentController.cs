using BaseClinic.Business.Services.Appointments.Commands;
using BaseClinic.Domain.Enums;
using BaseClinic.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BaseClinic.Domain.Constants;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize] // 1. Lớp bảo vệ vòng ngoài: Bắt buộc phải có JWT Token hợp lệ
    public class AppointmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AppointmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("book")]
        [RequirePermission(SystemPermissions.Appointment.Book)] // 2. Lớp bảo vệ vòng trong: Cần chính xác quyền này
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
        {
            var accountIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized(new { Message = "Token không hợp lệ hoặc không chứa thông tin định danh." });
            }

            var command = new BookAppointmentCommand
            {
                AccountId = accountId,
                DependentPatientId = request.DependentPatientId,
                NewDependentFullName = request.NewDependentFullName,
                NewDependentDob = request.NewDependentDob,
                NewDependentRelationship = request.NewDependentRelationship,
                DepartmentId = request.DepartmentId,
                RequestedDoctorId = request.RequestedDoctorId,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Reason = request.Reason
            };

            var appointmentId = await _mediator.Send(command);

            return Ok(new { AppointmentId = appointmentId, Message = "Đặt lịch khám thành công." });
        }

        public class BookAppointmentRequest
        {
            // Nhóm 1: Dành cho luồng "Đặt lịch cho người thân đã có hồ sơ"
            public Guid? DependentPatientId { get; set; }

            // Nhóm 2: Dành cho luồng "Tạo hồ sơ mới và đặt lịch cho người thân"
            public string? NewDependentFullName { get; set; }
            public DateTime? NewDependentDob { get; set; }
            public PatientRelationshipType? NewDependentRelationship { get; set; }

            // Nhóm 3: Thông tin chi tiết của lịch hẹn (Bắt buộc)
            public Guid DepartmentId { get; set; }
            public Guid? RequestedDoctorId { get; set; }
            public DateTime AppointmentDate { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
            public string Reason { get; set; } = string.Empty;
        }

        [HttpPost("check-in")]
        [RequirePermission(SystemPermissions.Appointment.CheckIn)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            // 1. Trích xuất ID định danh của Receptionist từ JWT Token
            var accountIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if(string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid receptionistId))
            {
                return Unauthorized(new { Message = "Token không hợp lệ hoặc không chứa thông tin định danh." });
            }

            // 2. Mapping sang Command
            var command = new CheckInCommand
            {
                ReceptionistId = receptionistId,
                AppointmentId = request.AppointmentId,
                PatientId = request.PatientId,
                DepartmentId = request.DepartmentId,
                NewPatientDob = request.NewPatientDob,
                NewPatientFullName = request.NewPatientFullName,
                IsPriority = request.IsPriority,
                Type = request.Type
            };

            // 3. Thực thi nghiệp vụ
            var result = await _mediator.Send(command);

            // 4. Trả về kết quả (thông tin để in phiếu khám)
            return Ok(new {Message = "Check-in thành công.", Data = result});
        }
        public class CheckInRequest
        {
            // Nhóm 1: Check-in theo lịch hẹn
            public Guid? AppointmentId { get; set; }

            // Nhóm 2: Walk-in bệnh nhân cũ
            public Guid? PatientId { get; set; }
            public Guid? DepartmentId { get; set; }

            // Nhóm 3: Walk-in bệnh nhân mới
            public string? NewPatientFullName { get; set; }
            public DateTime? NewPatientDob { get; set; }

            // Cấu hình xếp hàng
            public bool IsPriority { get; set; }
            public QueueType Type { get; set; } = QueueType.Consultation;
        }
    }
}
