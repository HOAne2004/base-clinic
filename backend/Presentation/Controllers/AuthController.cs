using BaseClinic.Business.Models.Auth;
using BaseClinic.Business.Services.Auth.Commands;
using BaseClinic.Presentation.Models.Requests;
using BaseClinic.Presentation.Models;
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
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Đăng ký thành công." });
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
