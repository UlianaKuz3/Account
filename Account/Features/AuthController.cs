using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Account.Features
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration config) : ControllerBase
    {

        /// <summary>
        /// Возвращает тестовый JWT токен для пользователя.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = GenerateJwtToken(request.Username);
            return Ok(token);
        }

        private string GenerateJwtToken(string username)
        {
            var jwtConfig = config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    /// <summary>
    /// Данные для логина.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Username { get; set; } = null;
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string? Password { get; set; } = null;
    }
}
