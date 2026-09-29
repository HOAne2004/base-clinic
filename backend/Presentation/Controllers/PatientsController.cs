using Azure.Core;
using BaseClinic.Business.Services.Patients.Commands;
using BaseClinic.Business.Services.Patients.Queries;
using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Enums;
using BaseClinic.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientsController(IMediator mediator)
        {
            _mediator = mediator;
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

        [HttpGet("my-dependents")]
        [RequirePermission(SystemPermissions.Patient.ManageDelegation)]
        // Hoặc có thể dùng Appointment.Book nếu luồng đặt lịch gọi API này
        public async Task<IActionResult> GetMyDependents()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetMyDependentsQuery(accountId));

            return Ok(result);
        }
    }
}
