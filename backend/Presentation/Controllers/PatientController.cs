using Azure.Core;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Business.Services.Patients.Commands;
using BaseClinic.Business.Services.Patients.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Presentation.Authorization;
using BaseClinic.Presentation.Models;
using BaseClinic.Presentation.Models.Requests.Patient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
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

        // ==================== LIST + SEARCH ====================
        [HttpGet]
        [RequirePermission("patient:view")]
        public async Task<IActionResult> GetList([FromQuery] PatientSearchDto search)
        {
            var result = await _mediator.Send(new GetPatientsQuery(search));
            return Ok(new ApiResponse<object>(result));
        }

        // ==================== GET ME (phải đặt TRƯỚC {id:guid}) ====================
        /// <summary>
        /// Lấy hồ sơ Patient primary của Account đang đăng nhập.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var accountId = GetAccountIdFromClaims();
            var result = await _mediator.Send(new GetPrimaryPatientQuery(accountId));

            if (result == null)
                throw new DomainException(
                    "Tài khoản này chưa có hồ sơ bệnh nhân.");

            return Ok(new ApiResponse<PatientDto>(result));
        }

        // ==================== DETAIL ====================
        [HttpGet("{id:guid}")]
        [RequirePermission("patient:view")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPatientByIdQuery(id));
            return Ok(new ApiResponse<object>(result));
        }

        // ==================== CREATE ====================
        [HttpPost]
        [RequirePermission("patient:create")]
        public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
        {
            var result = await _mediator.Send(new CreatePatientCommand(dto));

            // Nếu có DefaultPassword → FE hiển thị cho lễ tân
            var message = result.DefaultPassword != null
                ? $"Tạo thành công. Mật khẩu đăng nhập tạm: {result.DefaultPassword}. " +
                  $"Vui lòng thông báo cho bệnh nhân và yêu cầu đổi mật khẩu."
                : "Tạo hồ sơ bệnh nhân thành công.";

            return Ok(new ApiResponse<CreatePatientResultDto>(result, message));
        }

        // ==================== UPDATE ====================
        [HttpPut("{id:guid}")]
        [RequirePermission("patient:update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
        {
            await _mediator.Send(new UpdatePatientCommand(id, dto));
            return Ok(new ApiResponse<bool>(true, "Cập nhật hồ sơ thành công."));
        }

        // ==================== UPDATE ME ====================
        /// <summary>
        /// Patient tự cập nhật hồ sơ của mình.
        /// Chỉ cho phép sửa các field không phải định danh.
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileRequest request)
        {
            var accountId = GetAccountIdFromClaims();

            await _mediator.Send(new UpdateMyProfileCommand(
                AccountId: accountId,
                AvatarUrl: request.AvatarUrl,
                Address: request.Address));

            return Ok(new ApiResponse<bool>(true, "Cập nhật hồ sơ thành công."));
        }


        // ==================== MERGE ====================
        [HttpPost("merge")]
        [RequirePermission("patient:merge")]
        public async Task<IActionResult> Merge([FromBody] MergePatientRequest request)
        {
            var accountId = GetAccountIdFromClaims();

            var result = await _mediator.Send(new MergePatientCommand(
                SurvivorPatientId: request.SurvivorPatientId,
                MergedPatientId: request.MergedPatientId,
                Reason: request.Reason,
                MergedByAccountId: accountId));

            return Ok(new ApiResponse<MergePatientResultDto>(result,
                $"Đã gộp {result.MergedPatientCode} vào {result.SurvivorPatientCode}. " +
                $"Di chuyển: {result.AppointmentsMoved} lịch hẹn, " +
                $"{result.EncountersMoved} lượt khám, " +
                $"{result.DelegationsMoved} ủy quyền, " +
                $"{result.DependentsMoved} người phụ thuộc."));
        }

        // ==================== ROLLBACK REQUEST — REQUESTER ====================

        [HttpPost("merge-logs/{mergeLogId:guid}/rollback-requests")]
        [Authorize]
        public async Task<IActionResult> RequestRollback(
            Guid mergeLogId,
            [FromBody] RequestRollbackRequest request)
        {
            var accountId = GetAccountIdFromClaims();
            var id = await _mediator.Send(new RequestMergeRollbackCommand(
                PatientMergeLogId: mergeLogId,
                RequesterAccountId: accountId,
                Reason: request.Reason));

            return Ok(new ApiResponse<Guid>(id, "Đã gửi yêu cầu rollback."));
        }

        [HttpGet("rollback-requests/my")]
        [Authorize]
        public async Task<IActionResult> GetMyRollbackRequests()
        {
            var accountId = GetAccountIdFromClaims();
            var result = await _mediator.Send(new GetMyRollbackRequestsQuery(accountId));
            return Ok(new ApiResponse<List<MergeRollbackRequestDto>>(result));
        }

        [HttpPost("rollback-requests/{id:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelRollbackRequest(Guid id)
        {
            var accountId = GetAccountIdFromClaims();
            await _mediator.Send(new CancelMergeRollbackRequestCommand(id, accountId));
            return Ok(new ApiResponse<bool>(true, "Đã hủy yêu cầu."));
        }

        // ==================== ROLLBACK REQUEST — REVIEWER ====================

        [HttpGet("rollback-requests/pending")]
        [RequirePermission("patient:approve_rollback")]
        public async Task<IActionResult> GetPendingRollbackRequests()
        {
            var result = await _mediator.Send(new GetPendingRollbackRequestsQuery());
            return Ok(new ApiResponse<List<MergeRollbackRequestDto>>(result));
        }

        [HttpPost("rollback-requests/{id:guid}/approve")]
        [RequirePermission("patient:approve_rollback")]
        public async Task<IActionResult> ApproveRollback(
            Guid id,
            [FromBody] ApproveRollbackRequest request)
        {
            var accountId = GetAccountIdFromClaims();

            await _mediator.Send(new ApproveMergeRollbackCommand(
                RequestId: id,
                ReviewerAccountId: accountId,
                ReviewNote: request.ReviewNote,
                ConflictResolutions: request.ConflictResolutions));

            return Ok(new ApiResponse<bool>(true, "Đã duyệt và thực hiện rollback."));
        }

        [HttpPost("rollback-requests/{id:guid}/reject")]
        [RequirePermission("patient:approve_rollback")]
        public async Task<IActionResult> RejectRollback(
            Guid id,
            [FromBody] RejectRollbackRequest request)
        {
            var accountId = GetAccountIdFromClaims();

            await _mediator.Send(new RejectMergeRollbackCommand(
                RequestId: id,
                ReviewerAccountId: accountId,
                ReviewNote: request.ReviewNote));

            return Ok(new ApiResponse<bool>(true, "Đã từ chối yêu cầu."));
        }

        // ==================== DEPENDENTS ====================

        /// <summary>
        /// Danh sách người phụ thuộc của tôi.
        /// </summary>
        [HttpGet("me/dependents")]
        [Authorize]
        public async Task<IActionResult> GetMyDependents()
        {
            var accountId = GetAccountIdFromClaims();
            var result = await _mediator.Send(new GetMyDependentsQuery(accountId));
            return Ok(new ApiResponse<List<DependentListItemDto>>(result));
        }

        /// <summary>
        /// Tạo người phụ thuộc mới.
        /// </summary>
        [HttpPost("me/dependents")]
        [Authorize]
        public async Task<IActionResult> CreateDependent([FromBody] CreateDependentDto dto)
        {
            var accountId = GetAccountIdFromClaims();
            var result = await _mediator.Send(new CreateDependentCommand(accountId, dto));

            return Ok(new ApiResponse<CreateDependentResultDto>(
                result,
                $"Tạo người phụ thuộc thành công. Mã BN: {result.PatientCode}"));
        }

        /// <summary>
        /// Bỏ liên kết người phụ thuộc (không xóa hồ sơ).
        /// </summary>
        [HttpDelete("me/dependents/{dependentId:guid}")]
        [Authorize]
        public async Task<IActionResult> UnlinkDependent(Guid dependentId)
        {
            var accountId = GetAccountIdFromClaims();
            await _mediator.Send(new UnlinkDependentCommand(accountId, dependentId));
            return Ok(new ApiResponse<bool>(true, "Đã bỏ liên kết người phụ thuộc."));
        }





        [HttpPost("dependents")]
        [RequirePermission(SystemPermissions.Patient.ManageDelegation)]
        public async Task<IActionResult> AddDependent([FromBody] AddDependentRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized(new { Message = "Không thể xác thực danh tính người dùng." });
            }
            // Map dữ liệu sang Command
            var command = new AddDependentCommand(
                AccountId: accountId,
                FullName: request.FullName,
                PhoneNumber: request.PhoneNumber,
                DateOfBirth: request.DateOfBirth,
                Gender: request.Gender,
                RelationshipType: request.RelationshipType
            );

            var result = await _mediator.Send(command);
            return Ok(new { Message = result });
        }

        // DTO nhận dữ liệu từ Frontend (Không chứa AccountId)
        public record AddDependentRequest(
            string FullName,
            string PhoneNumber,
            DateTime? DateOfBirth,
            bool? Gender,
            PatientRelationshipType RelationshipType
        );

        [HttpPut("delegations/{delegationId}/respond")]
        [RequirePermission(SystemPermissions.Patient.ManageDelegation)]
        public async Task<IActionResult> RespondToDelegation(Guid delegationId, [FromBody] RespondDelegationRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized(new { Message = "Không thể xác thực danh tính người dùng." });
            }

            var command = new RespondToDelegationCommand(
                DelegationId: delegationId,
                AccountId: accountId,
                IsAccepted: request.IsAccepted
            );

            await _mediator.Send(command);

            var statusText = request.IsAccepted ? "chấp nhận" : "từ chối";
            return Ok(new { Message = $"Đã {statusText} yêu cầu liên kết hồ sơ." });
        }

        // Record nhận request body
        public record RespondDelegationRequest(bool IsAccepted);

        // Record nhận dữ liệu cho API Gửi mã
        public record RequestClaimOtpRequest(string PatientCode);

        // Record nhận dữ liệu cho API Xác nhận
        public record ConfirmClaimOtpRequest(string PatientCode, string Otp);


        [HttpPost("claim-profile/request-otp")]
        [RequirePermission(SystemPermissions.Patient.ManageDelegation)]
        public async Task<IActionResult> RequestClaimOtp([FromBody] RequestClaimOtpRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized(new { Message = "Không thể xác thực danh tính người dùng." });
            }

            var command = new RequestClaimProfileOtpCommand(accountId, request.PatientCode);
            var result = await _mediator.Send(command);

            // Ở môi trường thực tế, không trả về MockOtp
            return Ok(new
            {
                Message = $"Mã xác thực đã được gửi đến số điện thoại {result.MaskedPhoneNumber}.",
                MockOtp = result.MockOtp // Hỗ trợ test trên Postman/Swagger
            });
        }

        [HttpPost("claim-profile/confirm")]
        [RequirePermission(SystemPermissions.Patient.ManageDelegation)]
        public async Task<IActionResult> ConfirmClaimProfile([FromBody] ConfirmClaimOtpRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized(new { Message = "Không thể xác thực danh tính người dùng." });
            }

            var command = new ConfirmClaimProfileCommand(accountId, request.PatientCode, request.Otp);
            await _mediator.Send(command);

            return Ok(new { Message = "Liên kết hồ sơ thành công! Bạn đã có thể xem lịch sử khám của mình." });
        }

        [HttpDelete("delegations/{delegationId}/revoke")]
        [RequirePermission(SystemPermissions.Patient.ManageDelegation)]
        public async Task<IActionResult> RevokeDelegation(Guid delegationId)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized();
            }

            await _mediator.Send(new RevokeDelegationCommand(delegationId, accountId));

            return Ok(new { Message = "Đã thu hồi quyền truy cập hồ sơ thành công." });
        }
    }
}
