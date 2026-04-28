using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Infrastructure.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace InvokerTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IPlayerService service, IOptions<JwtOptions> options): ControllerBase
    {
        private readonly IPlayerService _PlayerService = service;
        private readonly IOptions<JwtOptions> _Options = options;

        private void AppDefaultCookieOptions(TokensPair tokensPair)
        {
            Response.Cookies.Append("aTo", tokensPair.accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, //!!!!
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(_Options.Value.Expires)
            });
            Response.Cookies.Append("rTo", tokensPair.refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, //!!!!
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(2)//test!!!!
            });
        }
        [HttpPost("registration")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequest registerationRequest)
        {
            await _PlayerService.Register(registerationRequest);
            var tokensPair = await _PlayerService.Login(new LoginRequest(registerationRequest.EmailOrPhoneNumber,registerationRequest.password));
            AppDefaultCookieOptions(tokensPair);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest LoginRequest)
        {
            var tokensPair = await _PlayerService.Login(LoginRequest);
            AppDefaultCookieOptions(tokensPair);
            return Ok();
        }
        [Route("refresh")]
        [HttpPost]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("rTo",out string? oldToken))
            {
                return Unauthorized();
            }
            var result = await _PlayerService.TryRefresh(oldToken);
            if(result != null)
            {
                AppDefaultCookieOptions(result);
                return Ok();
            }
            return Unauthorized();
        }
    }
}
