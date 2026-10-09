using BaseClinic.Business.Models.Auth;
using BaseClinic.Business.Services.Auth.Commands;
using BaseClinic.Business.Services.Auth.Queries;
using BaseClinic.Presentation.Models;
using BaseClinic.Presentation.Models.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace BaseClinic.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // ==================== HELPER: Đọc claims từ JWT ====================

        /// <summary>
        /// Đọc AccountId từ JWT claim. Throw 401 nếu không có.
        /// </summary>
        private Guid GetAccountIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(claim, out var accountId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");
            return accountId;
        }

        /// <summary>
        /// Đọc SessionId từ JWT claim. Throw 401 nếu không có.
        /// </summary>
        private Guid GetSessionIdFromClaims()
        {
            var claim = User.FindFirst("SessionId")?.Value;
            if (!Guid.TryParse(claim, out var sessionId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");
            return sessionId;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _mediator.Send(new RegisterCommand(
                FullName: request.FullName,
                PhoneNumber: request.PhoneNumber,
                Password: request.Password));

            return Ok(new ApiResponse<object>(
                result,
                "Đăng ký thành công. Vui lòng kiểm tra SMS để lấy mã OTP."));
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var deviceInfo = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new LoginCommand(
                dto.PhoneNumber,
                dto.Password,
                ipAddress,
                deviceInfo));

            return Ok(new ApiResponse<AuthResultDto>(result));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        // ==================== VERIFY OTP ====================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var deviceInfo = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new VerifyOtpCommand(
                AccountId: request.AccountId,
                OtpCode: request.OtpCode,
                IpAddress: ipAddress,
                DeviceInfo: deviceInfo));

            return Ok(new ApiResponse<object>(
                result,
                "Xác thực số điện thoại thành công."));
        }

        // ==================== RESEND OTP ====================
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var result = await _mediator.Send(new ResendOtpCommand(
                AccountId: request.AccountId));

            return Ok(new ApiResponse<object>(
                result,
                "Đã gửi lại mã OTP. Vui lòng kiểm tra SMS."));
        }

        // ==================== REFRESH TOKEN ====================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var deviceInfo = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new RefreshTokenCommand(
                RefreshToken: request.RefreshToken,
                IpAddress: ipAddress,
                DeviceInfo: deviceInfo));

            return Ok(new ApiResponse<object>(result));
        }

        // ==================== LOGOUT ====================
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Đọc từ JWT claim
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sessionIdClaim = User.FindFirst("SessionId")?.Value;

            if (!Guid.TryParse(accountIdClaim, out var accountId) ||
                !Guid.TryParse(sessionIdClaim, out var sessionId))
            {
                return Unauthorized(new
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Token không hợp lệ.",
                    Detail = "Không tìm thấy thông tin phiên đăng nhập."
                });
            }

            await _mediator.Send(new LogoutCommand(
                AccountId: accountId,
                SessionId: sessionId));

            return Ok(new ApiResponse<bool>(true, "Đăng xuất thành công."));
        }

        // ==================== LOGOUT ALL DEVICES ====================
        [HttpPost("logout-all-devices")]
        [Authorize]
        public async Task<IActionResult> LogoutAllDevices()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(accountIdClaim, out var accountId))
            {
                return Unauthorized(new
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Token không hợp lệ.",
                    Detail = "Không tìm thấy thông tin tài khoản."
                });
            }

            await _mediator.Send(new LogoutAllDevicesCommand(AccountId: accountId));

            return Ok(new ApiResponse<bool>(true,
                "Đã đăng xuất khỏi tất cả thiết bị. Vui lòng đăng nhập lại."));
        }

        // ==================== SESSION MANAGEMENT ====================

        /// <summary>
        /// Lấy danh sách thiết bị đang đăng nhập.
        /// </summary>
        [HttpGet("sessions")]
        [Authorize]
        public async Task<IActionResult> GetSessions()
        {
            var accountId = GetAccountIdFromClaims();
            var sessionId = GetSessionIdFromClaims();

            var result = await _mediator.Send(new GetSessionsQuery(
                AccountId: accountId,
                CurrentSessionId: sessionId));

            return Ok(new ApiResponse<List<Business.Models.Auth.SessionDto>>(result));
        }

        /// <summary>
        /// Thu hồi 1 session (đăng xuất thiết bị cụ thể).
        /// Có thể dùng để đăng xuất chính mình hoặc thiết bị khác.
        /// </summary>
        [HttpDelete("sessions/{sessionId}")]
        [Authorize]
        public async Task<IActionResult> RevokeSession(Guid sessionId)
        {
            var accountId = GetAccountIdFromClaims();

            await _mediator.Send(new RevokeSessionCommand(
                AccountId: accountId,
                SessionId: sessionId));

            return Ok(new ApiResponse<bool>(true,
                "Đã đăng xuất thiết bị thành công."));
        }
    }
}
