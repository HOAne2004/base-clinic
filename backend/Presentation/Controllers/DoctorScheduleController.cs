using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Business.Services.DoctorSchedule.Queries;
using BaseClinic.Business.Services.DoctorSchedules.Commands;
using BaseClinic.Business.Services.DoctorSchedules.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Presentation.Authorization;
using BaseClinic.Presentation.Models;
using BaseClinic.Presentation.Models.Requests.DoctorSchedule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/doctor-schedules")]
    [Authorize]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DoctorScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ==================== HELPERS ====================
        private Guid GetAccountIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(claim, out var accountId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");
            return accountId;
        }

        private bool IsAdmin()
        {
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            return roles.Contains(SystemRoles.Admin)
                || roles.Contains(SystemRoles.SuperAdmin);
        }

        // ==================== PUBLIC / VIEW ENDPOINTS ====================

        /// <summary>
        /// Xem slot còn chỗ — dùng cho đặt lịch (public).
        /// URL: GET /api/doctor-schedules/available-slots?workDate=...
        /// </summary>
        [HttpGet("available-slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] DateOnly workDate,
            [FromQuery] Guid? doctorId = null,
            [FromQuery] Guid? departmentId = null)
        {
            var result = await _mediator.Send(new GetAvailableSlotsQuery(
                WorkDate: workDate,
                DoctorId: doctorId,
                DepartmentId: departmentId));

            return Ok(new ApiResponse<List<AvailableSlotDto>>(result));
        }

        /// <summary>
        /// Chi tiết 1 ca làm việc (có slots).
        /// URL: GET /api/doctor-schedules/{id}
        /// </summary>
        [HttpGet("{id:guid}")]
        [RequirePermission(SystemPermissions.DoctorSchedule.View)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetScheduleByIdQuery(id));
            return Ok(new ApiResponse<DoctorScheduleDetailDto>(result));
        }

        /// <summary>
        /// Lịch làm việc của tôi (Doctor).
        /// URL: GET /api/doctor-schedules/me
        /// </summary>
        [HttpGet("me")]
        [RequirePermission(SystemPermissions.DoctorSchedule.View)]
        public async Task<IActionResult> GetMySchedules(
            [FromQuery] DateOnly? fromDate = null,
            [FromQuery] DateOnly? toDate = null)
        {
            var accountId = GetAccountIdFromClaims();

            var result = await _mediator.Send(new GetMySchedulesQuery(
                AccountId: accountId,
                FromDate: fromDate,
                ToDate: toDate));

            return Ok(new ApiResponse<List<DoctorScheduleListDto>>(result));
        }

        /// <summary>
        /// Lịch làm việc của 1 bác sĩ cụ thể.
        /// URL: GET /api/doctor-schedules/by-doctor/{doctorId}
        /// </summary>
        [HttpGet("by-doctor/{doctorId:guid}")]
        [RequirePermission(SystemPermissions.DoctorSchedule.View)]
        public async Task<IActionResult> GetByDoctor(
            Guid doctorId,
            [FromQuery] ScheduleSearchDto search)
        {
            var result = await _mediator.Send(new GetSchedulesByDoctorQuery(
                DoctorId: doctorId,
                Search: search));

            return Ok(new ApiResponse<object>(result));
        }

        /// <summary>
        /// Lịch làm việc của tất cả BS trong 1 khoa vào 1 ngày.
        /// URL: GET /api/doctor-schedules/day?departmentId=...&workDate=...
        /// </summary>
        [HttpGet("day")]
        [RequirePermission(SystemPermissions.DoctorSchedule.View)]
        public async Task<IActionResult> GetDaySchedule(
            [FromQuery] Guid departmentId,
            [FromQuery] DateOnly workDate)
        {
            var result = await _mediator.Send(new GetDoctorDayScheduleQuery(
                DepartmentId: departmentId,
                WorkDate: workDate));

            return Ok(new ApiResponse<DoctorDayScheduleDto>(result));
        }

        /// <summary>
        /// Danh sách tất cả ca làm việc (Admin).
        /// URL: GET /api/doctor-schedules
        /// </summary>
        [HttpGet]
        [RequirePermission(SystemPermissions.DoctorSchedule.View)]
        public async Task<IActionResult> GetAll([FromQuery] ScheduleSearchDto search)
        {
            var result = await _mediator.Send(new GetAllSchedulesQuery(search));
            return Ok(new ApiResponse<object>(result));
        }

        // ==================== CRUD ENDPOINTS ====================

        /// <summary>Tạo ca làm việc.</summary>
        [HttpPost]
        [RequirePermission(SystemPermissions.DoctorSchedule.Create)]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            var id = await _mediator.Send(new CreateScheduleCommand(dto));
            return Ok(new ApiResponse<Guid>(id, "Tạo ca làm việc thành công."));
        }

        /// <summary>Auto-fill nhiều ca.</summary>
        [HttpPost("auto-fill")]
        [RequirePermission(SystemPermissions.DoctorSchedule.AutoFill)]
        public async Task<IActionResult> AutoFill([FromBody] AutoFillScheduleDto dto)
        {
            var result = await _mediator.Send(new AutoFillScheduleCommand(dto));

            return Ok(new ApiResponse<AutoFillResultDto>(
                result,
                $"Đã tạo {result.CreatedSchedules} ca, " +
                $"bỏ qua {result.SkippedDays} ngày."));
        }

        /// <summary>Cập nhật ca làm việc.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(SystemPermissions.DoctorSchedule.Update)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateScheduleDto dto)
        {
            await _mediator.Send(new UpdateScheduleCommand(id, dto));
            return Ok(new ApiResponse<bool>(true, "Cập nhật ca làm việc thành công."));
        }

        /// <summary>Xóa ca làm việc (phải hết appointments).</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(SystemPermissions.DoctorSchedule.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteScheduleCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã xóa ca làm việc."));
        }

        // ==================== STATE ENDPOINTS ====================

        /// <summary>Đánh dấu ca hoàn thành.</summary>
        [HttpPost("{id:guid}/complete")]
        [RequirePermission(SystemPermissions.DoctorSchedule.Complete)]
        public async Task<IActionResult> Complete(Guid id)
        {
            await _mediator.Send(new CompleteScheduleCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã đánh dấu ca hoàn thành."));
        }

        /// <summary>Bật/tắt đặt lịch online cho ca.</summary>
        [HttpPatch("{id:guid}/online-booking")]
        [RequirePermission(SystemPermissions.DoctorSchedule.Update)]
        public async Task<IActionResult> SetOnlineBooking(
            Guid id,
            [FromBody] SetScheduleOnlineBookingRequest request)
        {
            await _mediator.Send(new SetScheduleOnlineBookingCommand(id, request.Enabled));

            var message = request.Enabled
                ? "Đã bật đặt lịch trực tuyến cho ca."
                : "Đã tắt đặt lịch trực tuyến cho ca.";

            return Ok(new ApiResponse<bool>(true, message));
        }

        // ==================== CANCEL ENDPOINTS ====================

        /// <summary>Hủy ca trực tiếp (≥ 3 ngày, hoặc Admin).</summary>
        [HttpPost("{id:guid}/cancel")]
        [RequirePermission(SystemPermissions.DoctorSchedule.Cancel)]
        public async Task<IActionResult> Cancel(
            Guid id,
            [FromBody] CancelScheduleDto dto)
        {
            var accountId = GetAccountIdFromClaims();
            var isAdmin = IsAdmin();

            var result = await _mediator.Send(new CancelScheduleCommand(
                ScheduleId: id,
                CancelledByAccountId: accountId,
                Reason: dto.Reason,
                IsAdmin: isAdmin));

            var message = result.CancelledAppointments > 0
                ? $"Đã hủy ca. {result.CancelledAppointments} appointment bị hủy."
                : "Đã hủy ca (không có appointment nào).";

            return Ok(new ApiResponse<CancelScheduleResultDto>(result, message));
        }

        /// <summary>Gửi yêu cầu hủy ca (< 3 ngày).</summary>
        [HttpPost("{id:guid}/cancel-requests")]
        [RequirePermission(SystemPermissions.DoctorSchedule.RequestCancel)]
        public async Task<IActionResult> RequestCancel(
            Guid id,
            [FromBody] RequestCancelScheduleDto dto)
        {
            var accountId = GetAccountIdFromClaims();

            var requestId = await _mediator.Send(new RequestCancelScheduleCommand(
                ScheduleId: id,
                RequesterAccountId: accountId,
                Reason: dto.Reason));

            return Ok(new ApiResponse<Guid>(
                requestId,
                "Đã gửi yêu cầu hủy ca. Vui lòng chờ Admin duyệt."));
        }

        // ==================== CANCEL REQUEST MANAGEMENT ====================

        /// <summary>Danh sách yêu cầu hủy đang chờ duyệt (Admin).</summary>
        [HttpGet("cancel-requests/pending")]
        [RequirePermission(SystemPermissions.DoctorSchedule.ApproveCancel)]
        public async Task<IActionResult> GetPendingRequests()
        {
            var result = await _mediator.Send(new GetPendingCancellationRequestsQuery());
            return Ok(new ApiResponse<List<ScheduleCancellationRequestDto>>(result));
        }

        /// <summary>Danh sách yêu cầu hủy của tôi.</summary>
        [HttpGet("cancel-requests/my")]
        [RequirePermission(SystemPermissions.DoctorSchedule.RequestCancel)]
        public async Task<IActionResult> GetMyRequests()
        {
            var accountId = GetAccountIdFromClaims();
            var result = await _mediator.Send(new GetMyCancellationRequestsQuery(accountId));
            return Ok(new ApiResponse<List<ScheduleCancellationRequestDto>>(result));
        }

        /// <summary>Duyệt yêu cầu hủy.</summary>
        [HttpPost("cancel-requests/{id:guid}/approve")]
        [RequirePermission(SystemPermissions.DoctorSchedule.ApproveCancel)]
        public async Task<IActionResult> ApproveRequest(
            Guid id,
            [FromBody] ReviewCancellationRequestDto dto)
        {
            var accountId = GetAccountIdFromClaims();

            var result = await _mediator.Send(new ApproveCancelScheduleCommand(
                RequestId: id,
                ReviewerAccountId: accountId,
                ReviewNote: dto.ReviewNote));

            var message = result.CancelledAppointments > 0
                ? $"Đã duyệt. {result.CancelledAppointments} appointment bị hủy."
                : "Đã duyệt yêu cầu hủy ca.";

            return Ok(new ApiResponse<CancelScheduleResultDto>(result, message));
        }

        /// <summary>Từ chối yêu cầu hủy.</summary>
        [HttpPost("cancel-requests/{id:guid}/reject")]
        [RequirePermission(SystemPermissions.DoctorSchedule.RejectCancel)]
        public async Task<IActionResult> RejectRequest(
            Guid id,
            [FromBody] RejectCancellationRequestDto dto)
        {
            var accountId = GetAccountIdFromClaims();

            await _mediator.Send(new RejectCancelScheduleCommand(
                RequestId: id,
                ReviewerAccountId: accountId,
                ReviewNote: dto.ReviewNote));

            return Ok(new ApiResponse<bool>(true, "Đã từ chối yêu cầu hủy ca."));
        }

        /// <summary>Requester tự hủy yêu cầu.</summary>
        [HttpPost("cancel-requests/{id:guid}/cancel")]
        [RequirePermission(SystemPermissions.DoctorSchedule.RequestCancel)]
        public async Task<IActionResult> CancelRequest(Guid id)
        {
            var accountId = GetAccountIdFromClaims();

            await _mediator.Send(new CancelCancellationRequestCommand(
                RequestId: id,
                RequesterAccountId: accountId));

            return Ok(new ApiResponse<bool>(true, "Đã hủy yêu cầu."));
        }

        // ==================== SLOT MANAGEMENT ====================

        /// <summary>Khóa 1 slot.</summary>
        [HttpPost("slots/{slotId:guid}/block")]
        [RequirePermission(SystemPermissions.DoctorSchedule.BlockSlot)]
        public async Task<IActionResult> BlockSlot(Guid slotId)
        {
            await _mediator.Send(new BlockSlotCommand(slotId));
            return Ok(new ApiResponse<bool>(true, "Đã khóa slot."));
        }

        /// <summary>Mở khóa slot.</summary>
        [HttpPost("slots/{slotId:guid}/unblock")]
        [RequirePermission(SystemPermissions.DoctorSchedule.BlockSlot)]
        public async Task<IActionResult> UnblockSlot(Guid slotId)
        {
            await _mediator.Send(new UnblockSlotCommand(slotId));
            return Ok(new ApiResponse<bool>(true, "Đã mở khóa slot."));
        }
    }
}