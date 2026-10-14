using BaseClinic.Business.Models.Doctor;
using BaseClinic.Business.Services.Doctors.Commands;
using BaseClinic.Business.Services.Doctors.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Presentation.Authorization;
using BaseClinic.Presentation.Models;
using BaseClinic.Presentation.Models.Requests.Doctor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DoctorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ==================== HELPER ====================
        private Guid GetAccountIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(claim, out var accountId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");
            return accountId;
        }

        // ==================== PUBLIC ENDPOINTS ====================
        // ⚠️ THỨ TỰ QUAN TRỌNG: Route literal (public, active, me)
        //    PHẢI đặt TRƯỚC route param ({slug}, {id:guid}).

        /// <summary>Danh sách bác sĩ công khai (landing page).</summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicList([FromQuery] DoctorSearchDto search)
        {
            var result = await _mediator.Send(new GetPublicDoctorsQuery(search));
            return Ok(new ApiResponse<object>(result));
        }

        /// <summary>Dropdown bác sĩ đang active + available for online.</summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveList([FromQuery] Guid? departmentId)
        {
            var result = await _mediator.Send(new GetActiveDoctorsQuery(departmentId));
            return Ok(new ApiResponse<List<DoctorLookupDto>>(result));
        }

        // ==================== SELF ENDPOINTS ====================
        // Đặt trước route {slug} để tránh match nhầm "me".

        /// <summary>Doctor tự xem profile của mình.</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var accountId = GetAccountIdFromClaims();
            var result = await _mediator.Send(new GetDoctorSelfProfileQuery(accountId));
            return Ok(new ApiResponse<DoctorSelfProfileDto>(result));
        }

        /// <summary>Doctor tự sửa profile (chỉ các field cho phép).</summary>
        [HttpPut("me")]
        [Authorize]
        [RequirePermission(SystemPermissions.Doctor.UpdateSelf)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateDoctorSelfProfileRequest request)
        {
            var accountId = GetAccountIdFromClaims();

            await _mediator.Send(new UpdateDoctorSelfProfileCommand(
                AccountId: accountId,
                Avatar: request.Avatar,
                Biography: request.Biography,
                Languages: request.Languages,
                Description: request.Description,
                IsAvailableForOnline: request.IsAvailableForOnline));

            return Ok(new ApiResponse<bool>(true, "Cập nhật hồ sơ thành công."));
        }

        // ==================== PUBLIC DETAIL (slug) ====================
        // Đặt CUỐI cùng để không conflict các route trên.

        /// <summary>
        /// Chi tiết bác sĩ công khai theo SLUG.
        /// VD: GET /api/doctors/bac-si-nguyen-van-a
        /// </summary>
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicProfile(string slug)
        {
            var result = await _mediator.Send(new GetDoctorPublicProfileQuery(slug));
            return Ok(new ApiResponse<DoctorPublicProfileDto>(result));
        }

        // ==================== ADMIN ENDPOINTS ====================

        /// <summary>Danh sách bác sĩ có phân trang (admin).</summary>
        [HttpGet]
        [RequirePermission(SystemPermissions.Doctor.View)]
        public async Task<IActionResult> GetList([FromQuery] DoctorSearchDto search)
        {
            var result = await _mediator.Send(new GetAllDoctorsQuery(search));
            return Ok(new ApiResponse<object>(result));
        }

        /// <summary>Chi tiết bác sĩ cho admin.</summary>
        [HttpGet("{id:guid}/admin")]
        [RequirePermission(SystemPermissions.Doctor.View)]
        public async Task<IActionResult> GetAdminDetail(Guid id)
        {
            var result = await _mediator.Send(new GetDoctorAdminDetailQuery(id));
            return Ok(new ApiResponse<DoctorAdminDetailDto>(result));
        }

        /// <summary>Tạo bác sĩ mới (tạo cả Account + Doctor).</summary>
        [HttpPost]
        [RequirePermission(SystemPermissions.Doctor.Create)]
        public async Task<IActionResult> Create([FromBody] CreateDoctorCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new ApiResponse<Guid>(id, "Tạo bác sĩ thành công."));
        }

        /// <summary>Cập nhật PROFILE (avatar, address, gender, DOB, CCCD, description).</summary>
        [HttpPut("{id:guid}/profile")]
        [RequirePermission(SystemPermissions.Doctor.Update)]
        public async Task<IActionResult> UpdateProfile(
            Guid id,
            [FromBody] UpdateDoctorProfileCommand command)
        {
            var finalCommand = command with { Id = id };
            await _mediator.Send(finalCommand);
            return Ok(new ApiResponse<bool>(true, "Cập nhật profile thành công."));
        }

        /// <summary>Cập nhật thông tin nghề nghiệp.</summary>
        [HttpPut("{id:guid}/professional-info")]
        [RequirePermission(SystemPermissions.Doctor.Update)]
        public async Task<IActionResult> UpdateProfessionalInfo(
            Guid id,
            [FromBody] UpdateDoctorProfessionalInfoCommand command)
        {
            var finalCommand = command with { Id = id };
            await _mediator.Send(finalCommand);
            return Ok(new ApiResponse<bool>(true, "Cập nhật thông tin nghề nghiệp thành công."));
        }

        /// <summary>Cập nhật workload (max patients, join date, online availability).</summary>
        [HttpPut("{id:guid}/workload")]
        [RequirePermission(SystemPermissions.Doctor.Update)]
        public async Task<IActionResult> UpdateWorkload(
            Guid id,
            [FromBody] UpdateDoctorWorkloadCommand command)
        {
            var finalCommand = command with { Id = id };
            await _mediator.Send(finalCommand);
            return Ok(new ApiResponse<bool>(true, "Cập nhật workload thành công."));
        }

        /// <summary>Cập nhật chứng chỉ hành nghề (license).</summary>
        [HttpPut("{id:guid}/license")]
        [RequirePermission(SystemPermissions.Doctor.Update)]
        public async Task<IActionResult> UpdateLicense(
            Guid id,
            [FromBody] UpdateDoctorLicenseCommand command)
        {
            var finalCommand = command with { Id = id };
            await _mediator.Send(finalCommand);
            return Ok(new ApiResponse<bool>(true, "Cập nhật chứng chỉ thành công."));
        }

        /// <summary>Chuyển khoa cho bác sĩ.</summary>
        [HttpPost("{id:guid}/transfer-department")]
        [RequirePermission(SystemPermissions.Doctor.TransferDepartment)]
        public async Task<IActionResult> TransferDepartment(
            Guid id,
            [FromBody] TransferDoctorDepartmentRequest request)
        {
            await _mediator.Send(new TransferDoctorDepartmentCommand(
                DoctorId: id,
                NewDepartmentId: request.NewDepartmentId));

            return Ok(new ApiResponse<bool>(true, "Chuyển khoa thành công."));
        }

        /// <summary>Kích hoạt lại bác sĩ.</summary>
        [HttpPost("{id:guid}/activate")]
        [RequirePermission(SystemPermissions.Doctor.Activate)]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivateDoctorCommand(id));
            return Ok(new ApiResponse<bool>(true, "Đã kích hoạt lại bác sĩ."));
        }

        /// <summary>Tạm ngưng bác sĩ.</summary>
        [HttpPost("{id:guid}/deactivate")]
        [RequirePermission(SystemPermissions.Doctor.Deactivate)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var result = await _mediator.Send(new DeactivateDoctorCommand(id));

            var message = result.Warnings.Any()
                ? "Đã tạm ngưng bác sĩ. Lưu ý: " + string.Join("; ", result.Warnings)
                : "Đã tạm ngưng bác sĩ.";

            return Ok(new ApiResponse<DeactivateDoctorResultDto>(result, message));
        }

        /// <summary>Xóa mềm bác sĩ (phải hết lịch hẹn).</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(SystemPermissions.Doctor.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var accountId = GetAccountIdFromClaims();
            await _mediator.Send(new DeleteDoctorCommand(id, accountId));
            return Ok(new ApiResponse<bool>(true, "Đã xóa bác sĩ."));
        }
    }
}