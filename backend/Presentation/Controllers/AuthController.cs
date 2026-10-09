using BaseClinic.Business.Models.Auth;
using BaseClinic.Business.Services.Auth.Commands;
using BaseClinic.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BaseClinic.Presentation.Models.Requests.Auth;


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

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Lấy ID người dùng hiện tại từ Token
            var accountIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(accountIdStr, out Guid accountId))
            {
                await _mediator.Send(new LogoutCommand(accountId));
            }
            return Ok(new { Message = "Đăng xuất thành công." });
        }
    }
}
