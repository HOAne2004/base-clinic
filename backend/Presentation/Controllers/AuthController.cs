using BaseClinic.Business.Services.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;


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
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token, Message = "Đăng nhập thành công." });
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { Message = "Đăng xuất thành công." });
        }
    }
}
