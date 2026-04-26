using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Infrastructure.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InvokerTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IPlayerService service, IOptions<JwtOptions> options): ControllerBase
    {
        private readonly IPlayerService _PlayerService = service;
        private readonly IOptions<JwtOptions> _Options = options;

        [HttpPost("registration")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequest registerationRequest)
        {
            await _PlayerService.Register(registerationRequest);
            var jwt = await _PlayerService.Login(new LoginRequest(registerationRequest.EmailOrPhoneNumber,registerationRequest.password));
            Response.Cookies.Append("my-cookie", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, //!!!!
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_Options.Value.Expires)
            });
            return Ok(new { token = jwt });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest LoginRequest)
        {
            var jwt = await _PlayerService.Login(LoginRequest);
            Response.Cookies.Append("my-cookie", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, //!!!!
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_Options.Value.Expires)
            });
            return Ok(new {token = jwt});
        }
    }
}
