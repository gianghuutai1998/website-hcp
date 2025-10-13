using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using hcp.Data;
using hcp.Helpers;

namespace hcp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ApiController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Vui lòng nhập đủ tài khoản và mật khẩu." });

            var user = _context.ApiUsers.FirstOrDefault(u =>
                u.Username == request.Username && u.IsActive);

            // ✅ Dùng PasswordHelper thay vì VerifyPassword trực tiếp
            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.Password ?? string.Empty))
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu!" });

            var token = GenerateJwtToken(user.Username);
            user.Token = token;
            _context.SaveChanges();

            return Ok(new { token });
        }

        private string GenerateJwtToken(string username)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "fallback-key";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "HCP",
                audience: "HCP",
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "ApiUser")
                },
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
