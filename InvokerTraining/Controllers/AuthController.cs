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
                Expires = DateTime.UtcNow.AddHours(_Options.Value.Refresh)
            });
        }
        [HttpPost("registration")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequest registerationRequest)
        {
            await _PlayerService.Register(registerationRequest);
            var tokensPair = await _PlayerService.Login(new LoginRequest(registerationRequest.EmailOrPhoneNumber,registerationRequest.password));
            if (!tokensPair.IsSuccess)
            {
                return BadRequest(tokensPair.Message);
            }
            AppDefaultCookieOptions(tokensPair.Value!);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest LoginRequest)
        {
            var tokensPair = await _PlayerService.Login(LoginRequest);
            if (!tokensPair.IsSuccess)
            {
                return BadRequest(tokensPair.Message);
            }
            AppDefaultCookieOptions(tokensPair.Value!);
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
                AppDefaultCookieOptions(result.Value!);
                return Ok();
            }
            return Unauthorized();
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var old = Request.Cookies["rTo"];
            if (old != null)
            {
                await _PlayerService.Logout(old);
                Response.Cookies.Delete("aTo");
                Response.Cookies.Delete("rTo");
                return Ok();
            }
            return BadRequest("Logout ERROR");
        }
    }
}
