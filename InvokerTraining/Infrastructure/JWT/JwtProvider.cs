using InvokerTraining.Application.Abstractions;
using InvokerTraining.Models.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InvokerTraining.Infrastructure.JWT
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;
        public JwtProvider(IOptions<JwtOptions> options)
        {
            _jwtOptions = options.Value;
        }
        public string GenerateAccessToken(Guid playerId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            Claim[] claims = [new Claim("userId", playerId.ToString())];
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtOptions.Expires),
                    signingCredentials: credentials);
            try
            {
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }

        public string GenerateRefreshToken() => Guid.NewGuid().ToString();
    }
}
